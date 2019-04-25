using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nakama;
using Newtonsoft.Json.Linq;

namespace NakamaMinimalGame.NakamaClient
{
    internal class GameManager
    {
        private IClient _client = new Client("defaultkey", "127.0.0.1", 7350, false);
        private ISession _session;
        private ISocket _socket;
        public FriendList.User CurrentUser { get; private set; }
        public bool IsConnected { get; private set; }
        private string _authtoken = "";

        public delegate void NotificationsHandler(IApiNotification note, Notifications e);
        public event NotificationsHandler OnNewNotifications;


        public FriendList FriendList { get; private set; }
        public GroupManager GroupManager { get; private set; }
        public MatchManager MatchManager { get; private set; }

        #region singleton
        private static GameManager _instance = null;
        private static readonly object Padlock = new object();
        GameManager()
        {
        }

        public static GameManager Instance
        {
            get
            {
                if (_instance != null) return _instance;
                lock (Padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new GameManager();
                    }
                }
                return _instance;
            }
        }

        #endregion

        public async Task Login(string email, string password, string username)
        {
            if (!string.IsNullOrEmpty(_authtoken))
            {
                var session = Session.Restore(_authtoken);
                if (!session.IsExpired)
                {
                    _session = session;
                }
            }
            if (_session == null)
            {
                _session = await _client.AuthenticateEmailAsync(email, password, username, true);
                _authtoken = _session.AuthToken;
            }
            if (_session == null)
                throw new Exception("Could not sign in");
            var account = await _client.GetAccountAsync(_session);
            CurrentUser = new FriendList.User {Id = account.User.Id, DisplayName = account.User.DisplayName, Username = account.User.Username};
            
            _socket = _client.CreateWebSocket();

            _socket.OnConnect += (sender, args) =>
            {
                IsConnected = true;
                FriendList = new FriendList(_session, _client, _socket);
                GroupManager = new GroupManager(_session, _client, _socket);
                MatchManager = new MatchManager(_session, _client, _socket);
                Console.WriteLine("Socket connected.");
            };
            _socket.OnDisconnect += (sender, args) =>
            {
                IsConnected = false;
                FriendList = null;
                GroupManager = null;
                MatchManager = null;
                Console.WriteLine("Socket disconnected.");
            };
            await _socket.ConnectAsync(_session);

            _socket.OnNotification += async (_, notification) =>
            {
                Console.WriteLine("Received notification {0}", notification);
                Console.WriteLine("Notification content {0}", notification.Content);
                var noteId = (Notifications) notification.Code;

                if (noteId == Notifications.FriendBlocked || noteId == Notifications.FriendDeleted || noteId == Notifications.FriendRequest || noteId == Notifications.FriendRequestAccepted || noteId == Notifications.RefreshFriendlist)
                {
                }
                else
                    OnNewNotifications?.Invoke(notification, noteId);
            };
            
        }
        public async Task Logoff()
        {
            await _socket.DisconnectAsync();
            _client = new Client("defaultkey", "127.0.0.1", 7350, false);
            _session = null;
            _socket = null;
            _authtoken = "";
            FriendList = null;
        }

        

        public enum Notifications
        {
            FriendHasJoinedTheGame = -6,
            GroupJoinRequest = -5,
            GroupJoinRequestAccepted = -4,
            FriendRequestAccepted = -3,
            FriendRequest = -2,
            WantToChat = -1,
            RefreshFriendlist = 1,
            FriendBlocked = 2,
            FriendDeleted = 3,
            RefreshGroup = 5
        }

        
    }
}
