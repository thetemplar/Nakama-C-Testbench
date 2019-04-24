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
                if ((GameManager.Notifications)notification.Code == GameManager.Notifications.RefreshFriendlist)
                    await GetFriendListFromServer();
            };

            _socket.OnStatusPresence += (_, presence) =>
            {
                Console.WriteLine("Presence {0}", presence);
                foreach (var leave in presence.Leaves)
                {

                    if (_friendList.ContainsKey(leave.UserId))
                    {
                        Console.WriteLine("User id '{0}' status gone '{1}'", leave.UserId, leave.Status);
                        _friendList[leave.UserId].User.Status = "";
                        _friendList[leave.UserId].User.Online = false;
                    }
                }

                foreach (var join in presence.Joins)
                {
                    if (_friendList.ContainsKey(join.UserId))
                    {
                        Console.WriteLine("User id '{0}' status update '{1}'", join.UserId, join.Status);
                        _friendList[join.UserId].User.Status = join.Status;
                        _friendList[join.UserId].User.Online = true;
                    }
                }
                
                UpdateFriendlist?.Invoke();
            };
            
            Task.Run(GetFriendListFromServer);
        }

        public class User
        {
            public string Id;
            public string Username;
            public string DisplayName;
            public string Status;
            public bool Online;
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

        public async Task GetFriendListFromServer()
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
                    var status = await _socket.FollowUsersAsync(new[] { friend.User.Id });
                    friend.User.Status = status.Presences.FirstOrDefault()?.Status;
                    change = true;
                }
                else
                {
                    _friendList[friend.User.Id] = friend;
                    change = true;
                }
            }

            foreach (string friend in _friendList.Keys.ToList())
            {
                // ReSharper disable once SimplifyLinqExpression
                if (!users.Any(n => n.User.Id == friend))
                {
                    await _socket.UnfollowUsersAsync(new[] { friend });
                    _friendList.Remove(friend);
                    change = true;
                }
            }

            Console.WriteLine("GetFriendListFromServer() - change: " + change);
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
