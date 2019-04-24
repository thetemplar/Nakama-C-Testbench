namespace NakamaMinimalGame
{
    partial class Game
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.tbEmail = new System.Windows.Forms.TextBox();
            this.btConnect = new System.Windows.Forms.Button();
            this.tbUsername = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.btAlternativeAccount = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label6 = new System.Windows.Forms.Label();
            this.tbStatus = new System.Windows.Forms.TextBox();
            this.lvFriend = new System.Windows.Forms.ListView();
            this.lvUser = new System.Windows.Forms.ListView();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cbInvisibleStatus = new System.Windows.Forms.CheckBox();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 7);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "email";
            // 
            // tbEmail
            // 
            this.tbEmail.Location = new System.Drawing.Point(64, 5);
            this.tbEmail.Margin = new System.Windows.Forms.Padding(2);
            this.tbEmail.Name = "tbEmail";
            this.tbEmail.Size = new System.Drawing.Size(76, 20);
            this.tbEmail.TabIndex = 1;
            this.tbEmail.Text = "dev@admin.de";
            // 
            // btConnect
            // 
            this.btConnect.Location = new System.Drawing.Point(20, 87);
            this.btConnect.Margin = new System.Windows.Forms.Padding(2);
            this.btConnect.Name = "btConnect";
            this.btConnect.Size = new System.Drawing.Size(94, 26);
            this.btConnect.TabIndex = 4;
            this.btConnect.Text = "Connect";
            this.btConnect.UseVisualStyleBackColor = true;
            this.btConnect.Click += new System.EventHandler(this.btConnect_Click);
            // 
            // tbUsername
            // 
            this.tbUsername.Location = new System.Drawing.Point(64, 51);
            this.tbUsername.Margin = new System.Windows.Forms.Padding(2);
            this.tbUsername.Name = "tbUsername";
            this.tbUsername.Size = new System.Drawing.Size(76, 20);
            this.tbUsername.TabIndex = 3;
            this.tbUsername.Text = "admin";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 54);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "username";
            // 
            // tbPassword
            // 
            this.tbPassword.Location = new System.Drawing.Point(64, 28);
            this.tbPassword.Margin = new System.Windows.Forms.Padding(2);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.Size = new System.Drawing.Size(76, 20);
            this.tbPassword.TabIndex = 2;
            this.tbPassword.Text = "developer";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 31);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "password";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(2);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(940, 534);
            this.tabControl1.TabIndex = 7;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.btAlternativeAccount);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.tbPassword);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.tbEmail);
            this.tabPage1.Controls.Add(this.tbUsername);
            this.tabPage1.Controls.Add(this.btConnect);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(2);
            this.tabPage1.Size = new System.Drawing.Size(932, 508);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Connect";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // btAlternativeAccount
            // 
            this.btAlternativeAccount.Location = new System.Drawing.Point(170, 4);
            this.btAlternativeAccount.Margin = new System.Windows.Forms.Padding(2);
            this.btAlternativeAccount.Name = "btAlternativeAccount";
            this.btAlternativeAccount.Size = new System.Drawing.Size(80, 20);
            this.btAlternativeAccount.TabIndex = 6;
            this.btAlternativeAccount.Text = "Fill with 2nd";
            this.btAlternativeAccount.UseVisualStyleBackColor = true;
            this.btAlternativeAccount.Click += new System.EventHandler(this.btAlternativeAccount_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.cbInvisibleStatus);
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Controls.Add(this.tbStatus);
            this.tabPage2.Controls.Add(this.lvFriend);
            this.tabPage2.Controls.Add(this.lvUser);
            this.tabPage2.Controls.Add(this.label5);
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(2);
            this.tabPage2.Size = new System.Drawing.Size(932, 508);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Lobby";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(444, 23);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(37, 13);
            this.label6.TabIndex = 7;
            this.label6.Text = "Status";
            // 
            // tbStatus
            // 
            this.tbStatus.Location = new System.Drawing.Point(445, 39);
            this.tbStatus.Name = "tbStatus";
            this.tbStatus.Size = new System.Drawing.Size(193, 20);
            this.tbStatus.TabIndex = 6;
            this.tbStatus.TextChanged += new System.EventHandler(this.tbStatus_TextChanged);
            // 
            // lvFriend
            // 
            this.lvFriend.Location = new System.Drawing.Point(166, 23);
            this.lvFriend.Margin = new System.Windows.Forms.Padding(2);
            this.lvFriend.MultiSelect = false;
            this.lvFriend.Name = "lvFriend";
            this.lvFriend.Size = new System.Drawing.Size(274, 482);
            this.lvFriend.TabIndex = 5;
            this.lvFriend.UseCompatibleStateImageBehavior = false;
            this.lvFriend.View = System.Windows.Forms.View.List;
            this.lvFriend.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lvFriend_MouseDown);
            // 
            // lvUser
            // 
            this.lvUser.Location = new System.Drawing.Point(8, 23);
            this.lvUser.Margin = new System.Windows.Forms.Padding(2);
            this.lvUser.MultiSelect = false;
            this.lvUser.Name = "lvUser";
            this.lvUser.Size = new System.Drawing.Size(154, 482);
            this.lvUser.TabIndex = 4;
            this.lvUser.UseCompatibleStateImageBehavior = false;
            this.lvUser.View = System.Windows.Forms.View.List;
            this.lvUser.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lvUser_MouseDown);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(164, 6);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(48, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "Friendlist";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 6);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(74, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "User @Server";
            // 
            // cbInvisibleStatus
            // 
            this.cbInvisibleStatus.AutoSize = true;
            this.cbInvisibleStatus.Location = new System.Drawing.Point(445, 65);
            this.cbInvisibleStatus.Name = "cbInvisibleStatus";
            this.cbInvisibleStatus.Size = new System.Drawing.Size(80, 17);
            this.cbInvisibleStatus.TabIndex = 8;
            this.cbInvisibleStatus.Text = "checkBox1";
            this.cbInvisibleStatus.UseVisualStyleBackColor = true;
            this.cbInvisibleStatus.CheckedChanged += new System.EventHandler(this.cbInvisibleStatus_CheckedChanged);
            // 
            // Game
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(940, 534);
            this.Controls.Add(this.tabControl1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Game";
            this.Text = "Start Game";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbEmail;
        private System.Windows.Forms.Button btConnect;
        private System.Windows.Forms.TextBox tbUsername;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbPassword;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListView lvFriend;
        private System.Windows.Forms.ListView lvUser;
        private System.Windows.Forms.Button btAlternativeAccount;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbStatus;
        private System.Windows.Forms.CheckBox cbInvisibleStatus;
    }
}

