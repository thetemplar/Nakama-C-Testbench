using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Nakama;

namespace NakamaMinimalGame.NakamaClient
{
    internal class GroupManager
    {
        private readonly ISession _session;
        private readonly ISocket _socket;
        private readonly IClient _client;
        GameManager _gm = GameManager.Instance;

        private readonly Dictionary<string, Group> _groups = new Dictionary<string, Group>();
        public IReadOnlyCollection<Group> Groups = new ReadOnlyCollection<Group>(new List<Group>());

        public delegate void UpdateGroupsHandler();
        public event UpdateGroupsHandler UpdateGroups;


        public class Group
        {
            public bool Open;
            public string Name;
            public string Description;
            public string Id;
            public List<GroupMember> Members;
            public class GroupMember
            {
                public enum GroupRole
                {
                    SuperAdmin = 0,
                    Admin = 1,
                    Member = 2,
                    JoinRequest = 3
                }
                public string MemberId;
                public GroupRole Role;
            }
        }

        public GroupManager(ISession session, IClient client, ISocket socket)
        {
            _session = session;
            _client = client;
            _socket = socket;

            _socket.OnNotification += async (_, notification) =>
            {
                if ((GameManager.Notifications)notification.Code == GameManager.Notifications.RefreshGroup)
                    await GetGroupsFromServer();
            };

            Task.Run(GetGroupsFromServer);
        }

        public async void CreateGroup(string name = "", string desc = "")
        {
            if (name == "")
                name = DateTime.Now.ToLongDateString() + "_" + DateTime.Now.ToLongTimeString();
            var group = await _client.CreateGroupAsync(_session, name, desc);
            Task.Run(GetGroupsFromServer);
        }

        public async Task GetGroupsFromServer()
        {
            var result = await _client.ListUserGroupsAsync(_session);
            List<Group> groups = new List<Group>();
            foreach (var ug in result.UserGroups)
            {
                Group grp = new Group
                {
                    Name = ug.Group.Name,
                    Description = ug.Group.Description,
                    Id = ug.Group.Id,
                    Open = ug.Group.Open,
                    Members = new List<Group.GroupMember>()
                };
                
                var resultGrpMember = await _client.ListGroupUsersAsync(_session, ug.Group.Id);
                foreach (var member in resultGrpMember.GroupUsers)
                {
                    grp.Members.Add(new Group.GroupMember{MemberId = member.User.Id, Role = (Group.GroupMember.GroupRole)member.State });
                }

                groups.Add(grp);
            }

            bool change = false;
            foreach (Group group in groups)
            {
                if (!_groups.ContainsKey(group.Id))
                {
                    _groups.Add(group.Id, group);
                    change = true;
                }
                else
                {
                    _groups[group.Id] = group;
                    change = true;
                }
            }

            foreach (string group in _groups.Keys.ToList())
            {
                // ReSharper disable once SimplifyLinqExpression
                if (!groups.Any(n => n.Id == group))
                {
                    _groups.Remove(group);
                    change = true;
                }
            }

            Console.WriteLine("GetGroupsFromServer() - change: " + change);
            
            if (change)
            {
                Groups = _groups.Values.ToList().AsReadOnly();
                UpdateGroups?.Invoke();
            }
        }

        public async void DeleteGroup(string groupId)
        {
            await _client.DeleteGroupAsync(_session, groupId);
        }
    }
}
