using System;
using System.Drawing;
using System.Windows.Forms;

namespace NakamaMinimalGame
{
    public partial class Game : Form
    {
        readonly GameManager _gm = GameManager.Instance;
        
        private readonly ContextMenuStrip _collectionRoundMenuStrip = new ContextMenuStrip();

        public Game()
        {
            InitializeComponent();
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

            //populate server-user-list
            FillUsers();
            FillFriends();

            tabControl1.SelectTab(1);

            tbEmail.Enabled = false;
            tbPassword.Enabled = false;
            tbUsername.Enabled = false;
            btConnect.Text = "Disconnect";
            btConnect.Click -= btConnect_Click;
            btConnect.Click += btConnect_Click_Disconnect;
        }
        private async void btConnect_Click_Disconnect(object sender, EventArgs e)
        {
            await _gm.Logoff();
            lvUser.Items.Clear();
            lvFriend.Items.Clear();

            tbEmail.Enabled = true;
            tbPassword.Enabled = true;
            tbUsername.Enabled = true;
            btConnect.Text = "Connect";
            btConnect.Click += btConnect_Click;
            btConnect.Click -= btConnect_Click_Disconnect;
        }

        private async void FillUsers()
        {
            var users = await _gm.GetUsers();
            lvUser.Items.Clear();
            foreach (var u in users)
            {
                lvUser.Items.Add(new ListViewItem { Text = u.DisplayName ?? u.Username, Tag = u.Id });
            }
        }
        private async void FillFriends()
        {
            var friends = await _gm.GetFriends();
            lvFriend.Items.Clear();
            foreach (var f in friends)
            {
                var username = f.User.DisplayName ?? f.User.Username;
                switch (f.State)
                {
                    case GameManager.Friend.FriendState.Friend:
                        if (f.User.Online)
                            lvFriend.Items.Add(new ListViewItem { Text = "[Online] " + username, Tag = f.User.Id });
                        else
                            lvFriend.Items.Add(new ListViewItem { Text = "[Offline] " + username, Tag = f.User.Id });
                        break;
                    case GameManager.Friend.FriendState.IncomingRequest:
                        lvFriend.Items.Add(new ListViewItem { Text = "[Requested] " + username, Tag = f.User.Id });
                        break;
                    case GameManager.Friend.FriendState.WaitingForAcceptance:
                        lvFriend.Items.Add(new ListViewItem { Text = "[Waiting] " + username, Tag = f.User.Id });
                        break;
                    case GameManager.Friend.FriendState.Banned:
                        lvFriend.Items.Add(new ListViewItem { Text = "[Banned] " + username, Tag = f.User.Id });
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
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
                    await _gm.AddAsFriend(item.Tag.ToString());
                    FillFriends();
                });
                _collectionRoundMenuStrip.Show(Cursor.Position);
                _collectionRoundMenuStrip.Visible = true;
                return;
            }
            _collectionRoundMenuStrip.Visible = false;            
        }
    }
}
