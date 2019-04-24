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
    class GroupManager
    {
        private readonly ISession _session;
        private readonly ISocket _socket;
        private readonly IClient _client;
        GameManager _gm = GameManager.Instance;

        public delegate void UpdateGroupsHandler();
        public event UpdateGroupsHandler UpdateGroups;

        Dictionary<string, Group> Groups = new Dictionary<string, Group>();

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
        }

        public async void CreateGroup(string name = "", string desc = "")
        {
            if (name == "")
                name = DateTime.Now.ToLongDateString() + "_" + DateTime.Now.ToLongTimeString();
            var group = await _client.CreateGroupAsync(_session, name, desc);
            System.Console.WriteLine("New group '{0}'", group.Id);
        }

        public async void ListGroup()
        {
            var result = await _client.ListUserGroupsAsync(_session);
            Groups.Clear();
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
                    var g = ug.Group;
                    System.Console.WriteLine("group '{0}' role '{1}'", g.Id, ug.State);
                    grp.Members.Add(new Group.GroupMember{MemberId = g.Id, Role = (Group.GroupMember.GroupRole)ug.State});
                }

                Groups.Add(grp.Id, grp);

            }
        }
    }
}
