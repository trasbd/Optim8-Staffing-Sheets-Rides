namespace O8SS_WebRequest
{
    partial class AboutForm
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

        private void InitializeComponent()
        {
            this.labelTitle = new System.Windows.Forms.Label();
            this.labelVersion = new System.Windows.Forms.Label();
            this.labelCopyright = new System.Windows.Forms.Label();
            this.buttonOK = new System.Windows.Forms.Button();
            this.linkWebsite = new System.Windows.Forms.LinkLabel();
            this.labelLicense = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelTitle
            // 
            this.labelTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.labelTitle.Location = new System.Drawing.Point(12, 9);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(360, 35);
            this.labelTitle.TabIndex = 0;
            this.labelTitle.Text = "Optim8 Staffing Sheets";
            this.labelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelVersion
            // 
            this.labelVersion.Location = new System.Drawing.Point(12, 44);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(360, 23);
            this.labelVersion.TabIndex = 1;
            this.labelVersion.Text = "Version 2025.07.12";
            this.labelVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelCopyright
            // 
            this.labelCopyright.Location = new System.Drawing.Point(12, 67);
            this.labelCopyright.Name = "labelCopyright";
            this.labelCopyright.Size = new System.Drawing.Size(360, 23);
            this.labelCopyright.TabIndex = 2;
            this.labelCopyright.Text = "© 2025 Thomas Robert";
            this.labelCopyright.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(147, 170);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(90, 30);
            this.buttonOK.TabIndex = 5;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // linkWebsite
            // 
            this.linkWebsite.Location = new System.Drawing.Point(12, 90);
            this.linkWebsite.Name = "linkWebsite";
            this.linkWebsite.Size = new System.Drawing.Size(360, 23);
            this.linkWebsite.TabIndex = 3;
            this.linkWebsite.TabStop = true;
            this.linkWebsite.Text = "https://github.com/trasbd/Optim8-Staffing-Sheets-Rides";
            this.linkWebsite.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.linkWebsite.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkWebsite_LinkClicked);
            // 
            // labelLicense
            // 
            this.labelLicense.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.labelLicense.Location = new System.Drawing.Point(12, 120);
            this.labelLicense.Name = "labelLicense";
            this.labelLicense.Size = new System.Drawing.Size(360, 40);
            this.labelLicense.TabIndex = 4;
            this.labelLicense.Text = "This software is licensed under the MIT License.\nSee GitHub for full details.";
            this.labelLicense.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // AboutForm
            // 
            this.AcceptButton = this.buttonOK;
            this.ClientSize = new System.Drawing.Size(384, 220);
            this.Controls.Add(this.labelTitle);
            this.Controls.Add(this.labelVersion);
            this.Controls.Add(this.labelCopyright);
            this.Controls.Add(this.linkWebsite);
            this.Controls.Add(this.labelLicense);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.Label labelCopyright;
        private System.Windows.Forms.LinkLabel linkWebsite;
        private System.Windows.Forms.Label labelLicense;
        private System.Windows.Forms.Button buttonOK;
    }
}
