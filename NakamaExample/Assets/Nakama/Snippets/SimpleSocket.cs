using Google.Protobuf;
using Nakama;
using NakamaMinimalGame.PublicMatchState;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SimpleSocket : MonoBehaviour
{
	private IClient _client = new Client("defaultkey", "127.0.0.1", 7350, false);
	private ISocket _socket;
    private ISession _session;
    private string _matchId;

    public PlayerController Player;
    public PlayerController ServerShadow;

    private DateTime _timeOfLastState;
    List<IUserPresence> _connectedOpponents = new List<IUserPresence>(0);

    private long _clientTick;
    private List<SendPackage> _notAcknowledgedPackages = new List<SendPackage>();

    private Dictionary<string, PlayerController> _npcs = new Dictionary<string, PlayerController>();

    public GameObject PrefabNPC;

    private async void Awake()
    {
        Screen.SetResolution(400, 400, false, 144);

        var deviceid = SystemInfo.deviceUniqueIdentifier;
        _session = await _client.AuthenticateDeviceAsync(deviceid + DateTime.Now.Second.ToString());
        
		_socket = _client.CreateWebSocket();
        _socket.OnConnect += _socket_OnConnect;
        _socket.OnDisconnect += _socket_OnDisconnect;

        await _socket.ConnectAsync(_session);
	}

    private void FixedUpdate()
    {
        if (string.IsNullOrEmpty(_matchId)) return;

        SendPackage send = new SendPackage
        {
            XAxis = Input.GetAxis("Horizontal"),
            YAxis = Input.GetAxis("Vertical"),
            Rotation = Player.Rotation,
            ClientTick = _clientTick
        };

        if (Player.Level >= PlayerController.LevelOfNetworking.B_Prediction)
            Player.ApplyPredictedInput(send.XAxis, send.YAxis, send.Rotation, Time.fixedDeltaTime);

        _notAcknowledgedPackages.Add(send);
        _socket.SendMatchState(_matchId, 0, send.ToByteArray());
        _clientTick++;
    }

    private void _socket_OnMatchState(object sender, IMatchState e)
    {
        PublicMatchState state = PublicMatchState.Parser.ParseFrom(e.State);

        var diffTime = (float)(DateTime.Now - _timeOfLastState).TotalSeconds;

        foreach (var player in state.Player)
        {
            //handle player character
            if (player.Key == _session.UserId)
            {
                _notAcknowledgedPackages.RemoveAll(x => x.ClientTick <= player.Value.LastProcessedClientTick);

                if(ServerShadow != null)
                    ServerShadow.SetLastServerAck(new Vector3(player.Value.Position.X, 1.5f, player.Value.Position.Y), player.Value.Rotation, null, diffTime);
                
                Player.SetLastServerAck(new Vector3(player.Value.Position.X, 1.5f, player.Value.Position.Y), player.Value.Rotation, _notAcknowledgedPackages, diffTime);
            }
            else
            {
                if(!_npcs.ContainsKey(player.Key))
                {
                    GameObject obj = Instantiate(PrefabNPC, new Vector3(player.Value.Position.X, 1.5f, player.Value.Position.Y), Quaternion.AngleAxis(player.Value.Rotation, Vector3.up));
                    _npcs.Add(player.Key, obj.GetComponent<PlayerController>());
                }
                _npcs[player.Key].SetLastServerAck(new Vector3(player.Value.Position.X, 1.5f, player.Value.Position.Y), player.Value.Rotation, _notAcknowledgedPackages, diffTime);
            }
        }

        _timeOfLastState = DateTime.Now;
    }


    private async void _socket_OnConnect(object sender, EventArgs e)
    {
        Debug.Log("Socket connected.");
        _socket.OnMatchmakerMatched += _socket_OnMatchmakerMatched;
        //_socket.AddMatchmakerAsync();

        var list = await _client.ListMatchesAsync(_session, 0, 10, 10, true, "");
        if(list.Matches.Count() == 0)
        {
            var match = await _client.RpcAsync(_session, "createMatch");
            _matchId = match.Payload;
        }
        else
        {
            _matchId = list.Matches.FirstOrDefault()?.MatchId;
        }

        await _socket.JoinMatchAsync(_matchId);
        Debug.Log("Created & joined match with ID: " + _matchId);
        _socket.OnMatchState += _socket_OnMatchState;

        _socket.OnMatchPresence += (_, presence) =>
        {
            _connectedOpponents.AddRange(presence.Joins);
            foreach (var leave in presence.Leaves)
            {
                _connectedOpponents.RemoveAll(item => item.SessionId.Equals(leave.SessionId));
            };
        };
    }

    private async void _socket_OnMatchmakerMatched(object sender, IMatchmakerMatched e)
    {
        Debug.Log("[OnMatchmakerMatched] Received MatchmakerMatched: " +  e.MatchId);
        var opponents = string.Join(",", e.Users.Select(x => x.Presence.Username)); // printable list.
        Debug.Log("[OnMatchmakerMatched] Matched opponents: " + opponents);
        IMatch match = await _socket.JoinMatchAsync(e);
        _matchId = match.Id;
    }

    private void _socket_OnDisconnect(object sender, EventArgs e)
    {
        Debug.Log("Socket disconnected.");
    }

    private async void OnApplicationQuit()
	{
		if (_socket != null)
        {
            if (!string.IsNullOrEmpty(_matchId))
            {
                var tmp_matchId = _matchId;
                _matchId = "";
                await _socket.LeaveMatchAsync(tmp_matchId);
            }
            await _socket.DisconnectAsync(false);
		}
	}
}
