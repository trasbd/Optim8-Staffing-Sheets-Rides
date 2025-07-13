using System;
using System.Windows.Forms;

namespace O8SS_WebRequest
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();

            DateTime buildDate = System.IO.File.GetLastWriteTime(
                System.Reflection.Assembly.GetExecutingAssembly().Location);

            labelVersion.Text = $"Version {buildDate:yyyy.MM.dd}";
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void linkWebsite_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = linkWebsite.Text,
                UseShellExecute = true
            });
        }

    }
}
