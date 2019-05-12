using Google.Protobuf;
using Nakama;
using NakamaMinimalGame.PublicMatchState;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;

public class SimpleSocket : MonoBehaviour
{
	private IClient _client = new Client("defaultkey", "127.0.0.1", 7350, false);
	private ISocket _socket;
    private ISession _session;
    private string _matchId;

    public PlayerController Player;
    public PlayerController ServerShadow;
    public UnitSelector UnitSelector;

    private DateTime _timeOfLastState;
    List<IUserPresence> _connectedOpponents = new List<IUserPresence>(0);

    private long _clientTick;
    private List<Client_Character> _notAcknowledgedPackages = new List<Client_Character>();

    private Dictionary<string, PlayerController> _gameObjects = new Dictionary<string, PlayerController>();

    public GameObject PrefabNPC;
    public GameObject PrefabTrainingBall;
    public GameObject PrefabFireball;
    public GameObject PrefabUnknown;


    private async void Awake()
    {
        UnityThread.initUnityThread();
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


        Client_Character send = new Client_Character
        {
            XAxis = Input.GetAxis("Horizontal") * (Input.GetMouseButton(1) ? 1 : 0) + (Input.GetKey("q") ? -1 : 0) + (Input.GetKey("e") ? 1 : 0),
            YAxis = Input.GetAxis("Vertical"),
            Rotation = Player.Rotation,
            ClientTick = _clientTick,
            Target = (UnitSelector.SelectedUnit != null) ? UnitSelector.SelectedUnit.name : "",
        };

        if (Player.Level >= PlayerController.LevelOfNetworking.B_Prediction)
            Player.ApplyPredictedInput(send.XAxis, send.YAxis, send.Rotation, Time.fixedDeltaTime);
        
        _notAcknowledgedPackages.Add(send);
        _socket.SendMatchState(_matchId, 0, send.ToByteArray());

        Client_Cast[] sendMsg = UnitSelector.GetCastMessages();
        foreach(var msg in sendMsg)
        {
            _socket.SendMatchState(_matchId, 1, msg.ToByteArray());
        }

        _clientTick++;            
    }

    private void _socket_OnMatchState(object sender, IMatchState e)
    {
        PublicMatchState state = PublicMatchState.Parser.ParseFrom(e.State);

        var diffTime = (float)(DateTime.Now - _timeOfLastState).TotalSeconds;

        //Debug.Log("Stopwatch-Server: 0:" + (state.Stopwatch[0] / 1000000f) + "ms 1>" + (state.Stopwatch[1] / 1000000f) + "ms 2>" + (state.Stopwatch[2] / 1000000f) + "ms 3>" + (state.Stopwatch[3] / 1000000f) + "ms 4>" + (state.Stopwatch[4] / 1000000f) + "ms");
        foreach (var player in state.Player)
        {
            //handle player character
            if (player.Key == _session.UserId)
            {
                _notAcknowledgedPackages.RemoveAll(x => x.ClientTick <= player.Value.LastProcessedClientTick);

                if(Player.ShowGhost)
                    ServerShadow.SetLastServerAck(new Vector3(player.Value.Position.X, 1.5f, player.Value.Position.Y), player.Value.Rotation, null, diffTime);
                
                Player.SetLastServerAck(new Vector3(player.Value.Position.X, 1.5f, player.Value.Position.Y), player.Value.Rotation, _notAcknowledgedPackages, diffTime);
            }
            else
            {
                if (_gameObjects.ContainsKey(player.Key))
                {
                    if (player.Value?.Position != null)
                    {
                        _gameObjects[player.Key].SetLastServerAck(new Vector3(player.Value.Position.X, 1.5f, player.Value.Position.Y), player.Value.Rotation, null, diffTime);
                    }
                    else
                    {
                        UnityThread.executeInUpdate(() =>
                        {
                            if (_gameObjects.ContainsKey(player.Key))
                            {
                                var del = _gameObjects.Where(x => x.Key == player.Key).FirstOrDefault();
                                Destroy(del.Value.gameObject);
                                _gameObjects.Remove(player.Key);
                            }
                        });
                    }
                }
                else
                {
                    if (player.Value?.Position != null)
                    {
                        UnityThread.executeInUpdate(() =>
                        {
                            if (!_gameObjects.ContainsKey(player.Key))
                            {
                                GameObject obj = Instantiate(PrefabNPC, new Vector3(player.Value.Position.X, 1.5f, player.Value.Position.Y), Quaternion.AngleAxis(player.Value.Rotation, Vector3.up));
                                obj.name = player.Key;
                                _gameObjects.Add(player.Key, obj.GetComponent<PlayerController>());
                            }
                        });
                    }
                }
            }
        }

        foreach (var npc in state.Npc)
        {
            if (_gameObjects.ContainsKey(npc.Key))
            {
                if (npc.Value?.Position != null)
                {
                    _gameObjects[npc.Key].SetLastServerAck(new Vector3(npc.Value.Position.X, 1.5f, npc.Value.Position.Y), npc.Value.Rotation, null, diffTime);
                }
                else
                {
                    UnityThread.executeInUpdate(() =>
                    {
                        if (_gameObjects.ContainsKey(npc.Key))
                        {
                            var del = _gameObjects.Where(x => x.Key == npc.Key).FirstOrDefault();
                            Destroy(del.Value.gameObject);
                            _gameObjects.Remove(npc.Key);
                        }
                    });
                }
            }
            else
            {
                if (npc.Value?.Position != null)
                {
                    UnityThread.executeInUpdate(() =>
                    {
                        if (!_gameObjects.ContainsKey(npc.Key))
                        {
                            GameObject toPlace = PrefabUnknown;
                            switch (npc.Value.Type)
                            {
                                case PublicMatchState.Types.NPC.Types.Type.Trainingball:
                                    toPlace = PrefabTrainingBall;
                                    break;
                            }
                            GameObject obj = Instantiate(toPlace, new Vector3(npc.Value.Position.X, 0f, npc.Value.Position.Y), Quaternion.AngleAxis(npc.Value.Rotation, Vector3.up));
                            obj.name = npc.Key;
                            _gameObjects.Add(npc.Key, obj.GetComponent<PlayerController>());
                        }
                    });
                }
            }
        }


        foreach (var projectile in _gameObjects.Where(x => x.Key.StartsWith("p_")))
        {
            if (!state.Projectile.ContainsKey(projectile.Key))
            {
                UnityThread.executeInUpdate(() =>
                {
                    if (_gameObjects.ContainsKey(projectile.Key))
                    {
                        var del = _gameObjects.Where(x => x.Key == projectile.Key).FirstOrDefault();
                        Destroy(del.Value.gameObject);
                        _gameObjects.Remove(projectile.Key);
                    }
                });
            }
        }

        foreach (var projectile in state.Projectile)
        {
            Debug.Log("projectile" + projectile.Value?.Position);
            if (_gameObjects.ContainsKey(projectile.Key))
            {
                if (projectile.Value != null && projectile.Value?.Position != null)
                {
                    _gameObjects[projectile.Key].SetLastServerAck(new Vector3(projectile.Value.Position.X, 1.5f, projectile.Value.Position.Y), projectile.Value.Rotation, null, diffTime);
                }
                else
                {
                    UnityThread.executeInUpdate(() =>
                    {
                        if (_gameObjects.ContainsKey(projectile.Key))
                        {
                            var del = _gameObjects.Where(x => x.Key == projectile.Key).FirstOrDefault();
                            Destroy(del.Value.gameObject);
                            _gameObjects.Remove(projectile.Key);
                        }
                    });
                }
            }
            else
            {
                if (projectile.Value?.Position != null)
                {
                    UnityThread.executeInUpdate(() =>
                    {
                        if (!_gameObjects.ContainsKey(projectile.Key))
                        {
                            GameObject toPlace = PrefabUnknown;
                            
                            switch (projectile.Value.Type)
                            {
                                case PublicMatchState.Types.Projectile.Types.Type.Fireball:
                                    toPlace = PrefabFireball;
                                    break;
                            }
                            GameObject obj = Instantiate(toPlace, new Vector3(projectile.Value.Position.X, 0f, projectile.Value.Position.Y), Quaternion.AngleAxis(projectile.Value.Rotation, Vector3.up));
                            obj.name = projectile.Key;
                            _gameObjects.Add(projectile.Key, obj.GetComponent<PlayerController>());
                        }
                    });
                }
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
