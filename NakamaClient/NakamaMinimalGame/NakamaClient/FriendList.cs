using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nakama;
using Newtonsoft.Json.Linq;

namespace NakamaMinimalGame.NakamaClient
{
    class FriendList
    {
        private readonly ISession _session;
        private readonly ISocket _socket;
        private readonly IClient _client;
        GameManager _gm = GameManager.Instance;
        private Dictionary<string, Friend> _friendList = new Dictionary<string, Friend>();
        public IReadOnlyCollection<Friend> Friends = new ReadOnlyCollection<Friend>(new List<Friend>());

        public delegate void UpdateFriendlistHandler();
        public event UpdateFriendlistHandler UpdateFriendlist;

        public FriendList(ISession session, IClient client, ISocket socket)
        {
            _session = session;
            _client = client;
            _socket = socket;

            _friendList = new Dictionary<string, Friend>();

            _socket.OnNotification += async (_, notification) =>
            {
                var noteId = (GameManager.Notifications)notification.Code;
                if (noteId == GameManager.Notifications.FriendBlocked || noteId == GameManager.Notifications.FriendDeleted || noteId == GameManager.Notifications.FriendRequest || noteId == GameManager.Notifications.FriendRequestAccepted)
                    await GetFriendListFromServer();
            };

            _socket.OnStatusPresence += (_, presence) =>
            {
                var join = presence.Joins.FirstOrDefault();
                if (join != null && _friendList.ContainsKey(join.UserId))
                {
                    _friendList[join.UserId].User.Status = join.Status;
                }
                UpdateFriendlist?.Invoke();
            };
            GetFriendListFromServer();
        }

        public class User
        {
            public string Id;
            public string Username;
            public string DisplayName;
            public bool Online;
            public string Status;
        }

        public class Friend
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


        public async Task<List<User>> GetUserlistFromServer()
        {
            if (_session == null)
                throw new Exception("No Login!");

            var res = await _client.RpcAsync(_session, "getPlayers");
            dynamic data = JArray.Parse(res.Payload);
            List<User> users = new List<User>();
            foreach (var d in data)
            {
                if (d != null && d.username != null && d.id != _gm.CurrentUser.Id)
                    users.Add(new User { Id = d.id, Username = d.username, DisplayName = d.displayname });
            }
            return users;
        }

        private async Task GetFriendListFromServer()
        {
            if (_session == null || _socket == null)
                throw new Exception("No Login!");

            var res = await _client.ListFriendsAsync(_session);
            List<Friend> users = new List<Friend>();
            foreach (var d in res.Friends)
            {
                users.Add(new Friend
                {
                    User = new User
                    {
                        Username = d.User.Username,
                        Online = d.User.Online,
                        DisplayName = d.User.DisplayName,
                        Id = d.User.Id
                    },
                    State = (Friend.FriendState)d.State
                });
            }

            bool change = false;
            foreach (Friend friend in users)
            {
                if(!_friendList.ContainsKey(friend.User.Id))
                {
                    _friendList.Add(friend.User.Id, friend);
                    await _socket.FollowUsersAsync(new[] { friend.User.Id });
                    change = true;
                }
                else
                {
                    if (_friendList[friend.User.Id].User.Status == friend.User.Status) continue;

                    _friendList[friend.User.Id].User.Status = friend.User.Status;
                    change = true;
                }
            }

            foreach (KeyValuePair<string, Friend> friend in _friendList)
            {
                // ReSharper disable once SimplifyLinqExpression
                if (!users.Any(n => n.User.Id == friend.Key))
                {
                    await _socket.UnfollowUsersAsync(new[] { friend.Key });
                    _friendList.Remove(friend.Key);
                    change = true;
                }
            }

            if (change)
            {
                Friends = _friendList.Values.ToList().AsReadOnly();
                UpdateFriendlist?.Invoke();
            }
        }


        public async Task AddAsFriend(string id)
        {
            if (_session == null)
                throw new Exception("No Login!");

            await _client.AddFriendsAsync(_session, new[] { id });
        }

        public async Task DeleteAsFriend(string id)
        {
            if (_session == null)
                throw new Exception("No Login!");

            await _client.DeleteFriendsAsync(_session, new[] { id });
        }

        public async Task BanAsFriend(string id)
        {
            if (_session == null)
                throw new Exception("No Login!");

            await _client.BlockFriendsAsync(_session, new[] { id });
        }

        public async Task ChangeStatus(string status)
        {
            if (_session == null)
                throw new Exception("No Login!");

            await _socket.UpdateStatusAsync(status);
        }
    }
}
