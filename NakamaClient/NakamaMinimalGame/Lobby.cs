using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Nakama;
using NakamaMinimalGame.NakamaClient;

namespace NakamaMinimalGame
{
    public partial class Lobby : Form
    {
        private readonly GameManager _gm = GameManager.Instance;
        
        private readonly ContextMenuStrip _collectionRoundMenuStrip = new ContextMenuStrip();
        private static readonly object Obj = new object();


        public Lobby()
        {
            InitializeComponent();
            tabControl1.TabPages[1].Enabled = false;
            tabControl1.TabPages[2].Enabled = false;

            _gm.OnNewNotifications += GmOnOnNewNotifications;
        }

        private void GmOnOnNewNotifications(IApiNotification note, GameManager.Notifications e)
        {
        }

        private void btAlternativeAccount_Click(object sender, EventArgs e)
        {
            tbEmail.Text = "second@test.com";
            tbPassword.Text = "secondbest";
            tbUsername.Text = "second_user";
        }

        private async void btConnect_Click(object sender, EventArgs e)
        {
            if (tbPassword.Text.Length > 8 && tbEmail.Text.Length > 9 && tbEmail.Text.Contains("@"))
                await _gm.Login(tbEmail.Text, tbPassword.Text, tbUsername.Text);


            this.Text = "Connected as " + (_gm.CurrentUser.DisplayName ?? _gm.CurrentUser.Username) + " -- " + _gm.CurrentUser.Id;

            tabControl1.TabPages[1].Enabled = true;
            tabControl1.TabPages[2].Enabled = true;
            tabControl1.SelectTab(2);

            tbEmail.Enabled = false;
            tbPassword.Enabled = false;
            tbUsername.Enabled = false;
            btConnect.Text = "Disconnect";
            btConnect.Click -= btConnect_Click;
            btConnect.Click += btConnect_Click_Disconnect;

            _gm.FriendList.UpdateFriendList += UpdateFriendList;
            _gm.GroupManager.UpdateGroups += UpdateGroupList;
            _gm.MatchManager.UpdateGameStatus += MatchManager_UpdateGameStatus;

            //populate server-user-list
            UpdateServerUsers();

        }

        private void MatchManager_UpdateGameStatus()
        {
            btCreateMatch.Enabled = !_gm.MatchManager.IsInMatch;
        }

        private async void btConnect_Click_Disconnect(object sender, EventArgs e)
        {
            await _gm.Logoff();
            lvUser.Items.Clear();
            lvFriend.Items.Clear();

            tabControl1.TabPages[1].Enabled = false;
            tabControl1.TabPages[2].Enabled = false;
            this.Text = "Disconnected";
            tbEmail.Enabled = true;
            tbPassword.Enabled = true;
            tbUsername.Enabled = true;
            btConnect.Text = "Connect";
            btConnect.Click += btConnect_Click;
            btConnect.Click -= btConnect_Click_Disconnect;
        }

        private void UpdateGroupList()
        {
            var groups = _gm.GroupManager.Groups;

            int c = 0;
            foreach (var group in groups)
            {
                if(tabGroups.TabPages.ContainsKey(group.Id)) continue;

                var newTabPage = new TabPage()
                {
                    Text = c.ToString(),
                    Name = group.Id
                };
                
                var lbName = new Label() { Text = group.Name, Location = new Point(10, 10), Size = new Size(200, 14) };
                newTabPage.Controls.Add(lbName);
                var lbId = new Label() { Text = group.Id, Location = new Point(10, 30), Size = new Size(200, 14) };
                newTabPage.Controls.Add(lbId);

                var lbMembers = new Label() { Location = new Point(10, 90), Size = new Size(200, 200) };
                StringBuilder sb = new StringBuilder();
                foreach (var member in group.Members)
                {
                    sb.Append(member.MemberId + Environment.NewLine);
                }
                lbMembers.Text = sb.ToString();
                newTabPage.Controls.Add(lbMembers);

                var btDel = new Button()
                {
                    Text = "Delete Group",
                    Location = new Point(10, 50)
                };
                btDel.Click += (sender, args) => { _gm.GroupManager.DeleteGroup(group.Id); };
                newTabPage.Controls.Add(btDel);

                tabGroups.Invoke((MethodInvoker)(() => tabGroups.TabPages.Insert(tabGroups.TabCount - 1, newTabPage)));
                c++;
            }
        }

        private async void UpdateServerUsers()
        {
            var users = await _gm.FriendList.GetUserlistFromServer();
            lvUser.Items.Clear();
            foreach (var u in users)
            {
                lvUser.Items.Add(new ListViewItem { Text = u.DisplayName ?? u.Username, Tag = u.Id });
            }
        }

        private void UpdateFriendList()
        {
            var friends = _gm.FriendList.Friends;
            lock (Obj)
            {
                Console.WriteLine("UpdateFriendList");
                lvFriend.Invoke((MethodInvoker)(() => lvFriend.Items.Clear()));
                foreach (var f in friends)
                {
                    var username = f.User.DisplayName ?? f.User.Username;
                    switch (f.State)
                    {
                        case FriendList.Friend.FriendState.Friend:
                            if (f.User.Online)
                                if(string.IsNullOrEmpty(f.User.Status))
                                    lvFriend.Invoke((MethodInvoker)(() => lvFriend.Items.Add(new ListViewItem { Text = "[Online] " + username, Tag = f.User.Id })));
                                else
                                    lvFriend.Invoke((MethodInvoker)(() => lvFriend.Items.Add(new ListViewItem { Text = "[" + f.User.Status + "] " + username, Tag = f.User.Id })));
                            else
                                lvFriend.Invoke((MethodInvoker)(() => lvFriend.Items.Add(new ListViewItem {Text = "[Offline] " + username, Tag = f.User.Id})));
                            break;
                        case FriendList.Friend.FriendState.IncomingRequest:
                            lvFriend.Invoke((MethodInvoker)(() => lvFriend.Items.Add(new ListViewItem {Text = "[Requested] " + username, Tag = f.User.Id})));
                            break;
                        case FriendList.Friend.FriendState.WaitingForAcceptance:
                            lvFriend.Invoke((MethodInvoker)(() => lvFriend.Items.Add(new ListViewItem {Text = "[Waiting] " + username, Tag = f.User.Id})));
                            break;
                        case FriendList.Friend.FriendState.Banned:
                            lvFriend.Invoke((MethodInvoker)(() => lvFriend.Items.Add(new ListViewItem {Text = "[Banned] " + username, Tag = f.User.Id})));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

        private void lvUser_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            _collectionRoundMenuStrip.Items.Clear();
            foreach (ListViewItem item in lvUser.Items)
            {
                if (!item.Bounds.Contains(new Point(e.X, e.Y))) continue;

                _collectionRoundMenuStrip.Items.Add("Add As Friend", null, async (send, args) =>
                {
                    await _gm.FriendList.AddAsFriend(item.Tag.ToString());
                });
                _collectionRoundMenuStrip.Show(Cursor.Position);
                _collectionRoundMenuStrip.Visible = true;
                return;
            }
            _collectionRoundMenuStrip.Visible = false;            
        }

        private void lvFriend_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            _collectionRoundMenuStrip.Items.Clear();
            foreach (ListViewItem item in lvFriend.Items)
            {
                if (!item.Bounds.Contains(new Point(e.X, e.Y))) continue;

                if (item.Text.StartsWith("[Requested]") || item.Text.StartsWith("[Waiting]"))
                {
                    _collectionRoundMenuStrip.Items.Add("Accept Friend", null, async (send, args) =>
                    {
                        await _gm.FriendList.AddAsFriend(item.Tag.ToString());
                    });
                    _collectionRoundMenuStrip.Items.Add("Refuse Friend", null, async (send, args) =>
                    {
                        await _gm.FriendList.DeleteAsFriend(item.Tag.ToString());
                    });
                    _collectionRoundMenuStrip.Items.Add("Ban Friend", null, async (send, args) =>
                    {
                        await _gm.FriendList.BanAsFriend(item.Tag.ToString());
                    });
                }
                if (item.Text.StartsWith("[Online]") || item.Text.StartsWith("[Offline]"))
                {
                    _collectionRoundMenuStrip.Items.Add("Delete Friend", null, async (send, args) =>
                    {
                        await _gm.FriendList.DeleteAsFriend(item.Tag.ToString());
                    });
                    _collectionRoundMenuStrip.Items.Add("Block Friend", null, async (send, args) =>
                    {
                        await _gm.FriendList.BanAsFriend(item.Tag.ToString());
                    });
                }
                if (item.Text.StartsWith("[Banned]"))
                {
                    _collectionRoundMenuStrip.Items.Add("Un-Ban Friend", null, async (send, args) =>
                    {
                        await _gm.FriendList.DeleteAsFriend(item.Tag.ToString());
                    });
                }

                _collectionRoundMenuStrip.Show(Cursor.Position);
                _collectionRoundMenuStrip.Visible = true;
                return;
            }
            _collectionRoundMenuStrip.Visible = false;
        }

        private async void tbStatus_TextChanged(object sender, EventArgs e)
        {
            await _gm.FriendList.ChangeStatus(tbStatus.Text);
            
        }

        private async void cbInvisibleStatus_CheckedChanged(object sender, EventArgs e)
        {
            if (cbInvisibleStatus.Checked)
            {
                tbStatus.Enabled = false;
                await _gm.FriendList.ChangeStatus(null);
            }
            else
            {
                tbStatus.Enabled = true;
                await _gm.FriendList.ChangeStatus(tbStatus.Text);
            }

        }

        private void btCreateGroup_Click(object sender, EventArgs e)
        {
            _gm.GroupManager.CreateGroup();
        }

        private async void btCreateMatch_Click(object sender, EventArgs e)
        {
            await _gm.MatchManager.CreateMatch();
        }

        private async void btMatchmaker_Click(object sender, EventArgs e)
        {
            if (!_gm.MatchManager.IsQueued)
            {
                await _gm.MatchManager.QueueMatchmaking();
                btMatchmaker.Text = "Cancel Search";
                btCreateMatch.Enabled = false;
            }
            else
            {
                await _gm.MatchManager.CancelMatchmaking();
                btMatchmaker.Text = "Find Match";
                btCreateMatch.Enabled = true;
            }
        }
    }
}
