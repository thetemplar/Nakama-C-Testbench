using Assets.Scripts.NakamaManager;
using Nakama;
using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Manager
{
    class NakamaManager : Singleton<NakamaManager>
    {
        [SerializeField] private readonly string _ipAddress = "localhost";
        [SerializeField] private readonly int _port = 7350;
        private string _deviceId;
        private Client _client;
        private ISocket _socket;

        public ISession Session { get; private set; }
        public IApiAccount Account { get; private set; }

        public Client Client
        {
            get
            {
                if (_client == null)
                {
                    _client = new Client("defaultkey", _ipAddress, _port, false);
                }
                return _client;
            }
        }

        public ISocket Socket
        {
            get
            {
                if (_socket == null)
                {
                    _socket = Client.CreateWebSocket();
                    _socket.OnDisconnect += OnDisconnect;
                }
                return _socket;
            }
        }

        public bool IsConnected
        {
            get
            {
                if (Session == null || Session.HasExpired(DateTime.UtcNow) == true)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);


            //if (_erasePlayerPrefsOnStart == true)
            {
                PlayerPrefs.SetString("nakama.authToken", "");
                PlayerPrefs.SetString("nakama.deviceId", "");
            }

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            ConnectSocketAsync();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        }

        private void OnDisconnect(object sender, EventArgs e)
        {
            Disconnect();
        }


        protected override void OnDestroy()
        {
            Disconnect();
        }

        public void Disconnect()
        {
            Debug.Log("Disconnected from Nakama");
            Session = null;
            Account = null;
        }

        public async Task<bool> ConnectSocketAsync()
        {
            try
            {
                if (_socket != null)
                {
                    await _socket.DisconnectAsync();
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("Couldn't disconnect the socket: " + e);
            }

            Session = await Client.AuthenticateDeviceAsync("testtesttest" + DateTime.Now.Second.ToString());
            try
            {
                await Socket.ConnectAsync(Session);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError("An error has occured while connecting socket: " + e);
                return false;
            }
        }


        public async Task<string> StartOrJoinGameAsync()
        {
            Debug.Log("ListMatchesAsync ->");
            //var list = await Client.ListMatchesAsync(Session, 0, 10, 10, true, "");
            //Debug.Log("ListMatchesAsync l=" + list.Matches.Count());
            //if (list.Matches.Count() == 0)
            {
                var match = await _client.RpcAsync(Session, "createMatch");
                return match.Payload;
            }
            //else
            {
                //return list.Matches.FirstOrDefault()?.MatchId;
            }
        }

        public async Task<string> GetGameDatabase()
        {
            var res = await _client.RpcAsync(Session, "getGameDatabase", "");
            return res.Payload;
        }
    }
}
