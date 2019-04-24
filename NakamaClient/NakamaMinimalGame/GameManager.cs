using Nakama;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NakamaMinimalGame
{
    internal class GameManager
    {
        private IClient _client = new Client("defaultkey", "127.0.0.1", 7350, false);
        private ISession _session;
        string _authtoken = "";
        
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
            Console.WriteLine("User id '{0}'", account.User.Id);
            Console.WriteLine("User username '{0}'", account.User.Username);
        }
        public async Task Logoff()
        {
            _client = new Client("defaultkey", "127.0.0.1", 7350, false);
            _session = null;
            _authtoken = "";
        }

        public async Task<List<User>> GetUsers()
        {
            if (_session == null)
                throw new Exception("No Login!");

            var res = await _client.RpcAsync(_session, "getPlayers");
            dynamic data = JArray.Parse(res.Payload);
            List<User> users = new List<User>();
            foreach (var d in data)
            {
                if (d != null && d.username != null)
                    users.Add(new User { Id = d.id, Username = d.username, DisplayName = d.displayname });
            }
            return users;
        }

        public struct User
        {
            public string Id;
            public string Username;
            public string DisplayName;
            public bool Online;
        }
        public struct Friend
        {
            public enum FriendState
            {
                Friend = 0,
                WaitingForAcceptance = 1,
                IncomingRequest = 2,
                Banned = 3
            }
            public User User;
            public FriendState State;
        }
        
        public async Task<List<Friend>> GetFriends()
        {
            if (_session == null)
                throw new Exception("No Login!");

            var res = await _client.ListFriendsAsync(_session);
            List<Friend> users = new List<Friend>();
            foreach (var d in res.Friends)
            {
                users.Add(new Friend { User = new User { Username = d.User.Username, Online = d.User.Online, DisplayName = d.User.DisplayName, Id = d.User.Id}, State = (Friend.FriendState)d.State });
            }
            return users;
        }

        public async Task AddAsFriend(string id)
        {
            if (_session == null)
                throw new Exception("No Login!");

            await _client.AddFriendsAsync(_session, new[] { id });
        }
    }
}
