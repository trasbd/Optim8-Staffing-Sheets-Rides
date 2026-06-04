namespace O8SS_WebRequest
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtPass = new System.Windows.Forms.TextBox();
            this.txtID = new System.Windows.Forms.TextBox();
            this.txtCompany = new System.Windows.Forms.TextBox();
            this.LoginButton = new System.Windows.Forms.Button();
            this.AreaComboBox = new System.Windows.Forms.ComboBox();
            this.AreaLabel = new System.Windows.Forms.Label();
            this.dtpDate = new System.Windows.Forms.DateTimePicker();
            this.GoButton = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.SortOptionsButton = new System.Windows.Forms.Button();
            this.checkBoxRememberMe = new System.Windows.Forms.CheckBox();
            this.checkBoxPS = new System.Windows.Forms.CheckBox();
            this.checkBoxRestrooms = new System.Windows.Forms.CheckBox();
            this.AddlACOptionsButton = new System.Windows.Forms.Button();
            this.NotesCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 18);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 16);
            this.label3.TabIndex = 31;
            this.label3.Text = "Company:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 85);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 16);
            this.label2.TabIndex = 30;
            this.label2.Text = "Password:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 51);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 16);
            this.label1.TabIndex = 29;
            this.label1.Text = "Username:";
            // 
            // txtPass
            // 
            this.txtPass.Location = new System.Drawing.Point(93, 81);
            this.txtPass.Margin = new System.Windows.Forms.Padding(4);
            this.txtPass.Name = "txtPass";
            this.txtPass.PasswordChar = '*';
            this.txtPass.Size = new System.Drawing.Size(132, 22);
            this.txtPass.TabIndex = 2;
            // 
            // txtID
            // 
            this.txtID.Location = new System.Drawing.Point(93, 48);
            this.txtID.Margin = new System.Windows.Forms.Padding(4);
            this.txtID.Name = "txtID";
            this.txtID.Size = new System.Drawing.Size(132, 22);
            this.txtID.TabIndex = 1;
            // 
            // txtCompany
            // 
            this.txtCompany.Location = new System.Drawing.Point(93, 14);
            this.txtCompany.Margin = new System.Windows.Forms.Padding(4);
            this.txtCompany.Name = "txtCompany";
            this.txtCompany.Size = new System.Drawing.Size(132, 22);
            this.txtCompany.TabIndex = 4;
            this.txtCompany.Text = "EPSL";
            // 
            // LoginButton
            // 
            this.LoginButton.Location = new System.Drawing.Point(93, 137);
            this.LoginButton.Margin = new System.Windows.Forms.Padding(4);
            this.LoginButton.Name = "LoginButton";
            this.LoginButton.Size = new System.Drawing.Size(100, 28);
            this.LoginButton.TabIndex = 3;
            this.LoginButton.Text = "Login";
            this.LoginButton.UseVisualStyleBackColor = true;
            this.LoginButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // AreaComboBox
            // 
            this.AreaComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.AreaComboBox.FormattingEnabled = true;
            this.AreaComboBox.Location = new System.Drawing.Point(327, 102);
            this.AreaComboBox.Name = "AreaComboBox";
            this.AreaComboBox.Size = new System.Drawing.Size(222, 24);
            this.AreaComboBox.TabIndex = 33;
            this.AreaComboBox.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // AreaLabel
            // 
            this.AreaLabel.AutoSize = true;
            this.AreaLabel.Location = new System.Drawing.Point(281, 105);
            this.AreaLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.AreaLabel.Name = "AreaLabel";
            this.AreaLabel.Size = new System.Drawing.Size(39, 16);
            this.AreaLabel.TabIndex = 34;
            this.AreaLabel.Text = "Area:";
            // 
            // dtpDate
            // 
            this.dtpDate.Location = new System.Drawing.Point(284, 14);
            this.dtpDate.Margin = new System.Windows.Forms.Padding(4);
            this.dtpDate.Name = "dtpDate";
            this.dtpDate.Size = new System.Drawing.Size(265, 22);
            this.dtpDate.TabIndex = 35;
            // 
            // GoButton
            // 
            this.GoButton.Location = new System.Drawing.Point(341, 168);
            this.GoButton.Margin = new System.Windows.Forms.Padding(4);
            this.GoButton.Name = "GoButton";
            this.GoButton.Size = new System.Drawing.Size(100, 28);
            this.GoButton.TabIndex = 36;
            this.GoButton.Text = "Go";
            this.GoButton.UseVisualStyleBackColor = true;
            this.GoButton.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(449, 169);
            this.button3.Margin = new System.Windows.Forms.Padding(4);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(100, 28);
            this.button3.TabIndex = 37;
            this.button3.Text = "About";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // SortOptionsButton
            // 
            this.SortOptionsButton.Location = new System.Drawing.Point(341, 133);
            this.SortOptionsButton.Margin = new System.Windows.Forms.Padding(4);
            this.SortOptionsButton.Name = "SortOptionsButton";
            this.SortOptionsButton.Size = new System.Drawing.Size(100, 28);
            this.SortOptionsButton.TabIndex = 38;
            this.SortOptionsButton.Text = "Sort Options";
            this.SortOptionsButton.UseVisualStyleBackColor = true;
            this.SortOptionsButton.Click += new System.EventHandler(this.button4_Click);
            // 
            // checkBoxRememberMe
            // 
            this.checkBoxRememberMe.AutoSize = true;
            this.checkBoxRememberMe.Location = new System.Drawing.Point(93, 110);
            this.checkBoxRememberMe.Name = "checkBoxRememberMe";
            this.checkBoxRememberMe.Size = new System.Drawing.Size(119, 20);
            this.checkBoxRememberMe.TabIndex = 39;
            this.checkBoxRememberMe.Text = "Remember Me";
            this.checkBoxRememberMe.UseVisualStyleBackColor = true;
            this.checkBoxRememberMe.CheckedChanged += new System.EventHandler(this.checkBoxRememberMe_CheckedChanged);
            // 
            // checkBoxPS
            // 
            this.checkBoxPS.AutoSize = true;
            this.checkBoxPS.Enabled = false;
            this.checkBoxPS.Location = new System.Drawing.Point(284, 50);
            this.checkBoxPS.Name = "checkBoxPS";
            this.checkBoxPS.Size = new System.Drawing.Size(113, 20);
            this.checkBoxPS.TabIndex = 40;
            this.checkBoxPS.Text = "Park Services";
            this.checkBoxPS.UseVisualStyleBackColor = true;
            this.checkBoxPS.CheckedChanged += new System.EventHandler(this.checkBoxPS_CheckedChanged);
            // 
            // checkBoxRestrooms
            // 
            this.checkBoxRestrooms.AutoSize = true;
            this.checkBoxRestrooms.Checked = true;
            this.checkBoxRestrooms.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxRestrooms.Enabled = false;
            this.checkBoxRestrooms.Location = new System.Drawing.Point(403, 50);
            this.checkBoxRestrooms.Name = "checkBoxRestrooms";
            this.checkBoxRestrooms.Size = new System.Drawing.Size(152, 20);
            this.checkBoxRestrooms.TabIndex = 41;
            this.checkBoxRestrooms.Text = "Restroom into Home";
            this.checkBoxRestrooms.UseVisualStyleBackColor = true;
            this.checkBoxRestrooms.Visible = false;
            // 
            // AddlACOptionsButton
            // 
            this.AddlACOptionsButton.Location = new System.Drawing.Point(449, 133);
            this.AddlACOptionsButton.Margin = new System.Windows.Forms.Padding(4);
            this.AddlACOptionsButton.Name = "AddlACOptionsButton";
            this.AddlACOptionsButton.Size = new System.Drawing.Size(100, 28);
            this.AddlACOptionsButton.TabIndex = 42;
            this.AddlACOptionsButton.Text = "Add\'l AC";
            this.AddlACOptionsButton.UseVisualStyleBackColor = true;
            this.AddlACOptionsButton.Click += new System.EventHandler(this.button5_Click);
            // 
            // NotesCheckBox
            // 
            this.NotesCheckBox.AutoSize = true;
            this.NotesCheckBox.Checked = true;
            this.NotesCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.NotesCheckBox.Enabled = false;
            this.NotesCheckBox.Location = new System.Drawing.Point(284, 76);
            this.NotesCheckBox.Name = "NotesCheckBox";
            this.NotesCheckBox.Size = new System.Drawing.Size(111, 20);
            this.NotesCheckBox.TabIndex = 43;
            this.NotesCheckBox.Text = "Include Notes";
            this.NotesCheckBox.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AcceptButton = this.LoginButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(569, 206);
            this.Controls.Add(this.NotesCheckBox);
            this.Controls.Add(this.AddlACOptionsButton);
            this.Controls.Add(this.checkBoxRestrooms);
            this.Controls.Add(this.checkBoxPS);
            this.Controls.Add(this.checkBoxRememberMe);
            this.Controls.Add(this.SortOptionsButton);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.GoButton);
            this.Controls.Add(this.dtpDate);
            this.Controls.Add(this.AreaLabel);
            this.Controls.Add(this.AreaComboBox);
            this.Controls.Add(this.LoginButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtPass);
            this.Controls.Add(this.txtID);
            this.Controls.Add(this.txtCompany);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Staffing Sheets";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtPass;
        private System.Windows.Forms.TextBox txtID;
        private System.Windows.Forms.TextBox txtCompany;
        private System.Windows.Forms.Button LoginButton;
        private System.Windows.Forms.ComboBox AreaComboBox;
        private System.Windows.Forms.Label AreaLabel;
        private System.Windows.Forms.DateTimePicker dtpDate;
        private System.Windows.Forms.Button GoButton;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button SortOptionsButton;
        private System.Windows.Forms.CheckBox checkBoxRememberMe;
        private System.Windows.Forms.CheckBox checkBoxPS;
        private System.Windows.Forms.CheckBox checkBoxRestrooms;
        private System.Windows.Forms.Button AddlACOptionsButton;
        private System.Windows.Forms.CheckBox NotesCheckBox;
    }
}

