using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace O8SS_WebRequest
{
    public partial class Form1 : Form
    {
        private readonly ScheduleService _service;

        private string _LocaitonsFileName = "SortOrder.txt";
        private BindingList<string> Locations = new BindingList<string>();

        private bool _loggedIn;
        public bool LoggedIn
        {
            get => _loggedIn;
            set
            {
                _loggedIn = value;
                UpdateUiState();
            }
        }

        public Form1()
        {
            InitializeComponent();

            LoggedIn = false;

            CookieContainer cookies = new CookieContainer();
            HttpClientHandler handler = new HttpClientHandler
            {
                CookieContainer = cookies,
                UseCookies = true,
                AllowAutoRedirect = true
            };

            var client = new HttpClient(handler);
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
            client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");

            _service = new ScheduleService(client);


            string exeDir = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(exeDir, _LocaitonsFileName);

            if (File.Exists(filePath))
            {
                Locations = new BindingList<string>(File.ReadAllLines(filePath).ToList());
            }

        }

        private async void button1_Click(object sender, EventArgs e)
        {


            if (string.IsNullOrWhiteSpace(txtCompany.Text) ||
                    string.IsNullOrWhiteSpace(txtID.Text) ||
                    string.IsNullOrWhiteSpace(txtPass.Text))
            {
                MessageBox.Show("Please fill out all fields.", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (await _service.PerformLoginAsync(txtCompany.Text, txtID.Text, txtPass.Text))
            {
                //MessageBox.Show("Login successful!");

                var areas = (await _service.FetchAreas())
                    .OrderBy(area =>
                    {
                        if (area.Value.Contains("Rides Area"))
                            return 0;
                        else if (area.Value.Contains("Rides"))
                            return 1;
                        else
                            return 2;
                    })
                    .ThenBy(area => area.Value) // alphabetical within groups
                    .ToList();

                // Now populate the ComboBox
                comboBox1.DisplayMember = "Value";
                comboBox1.ValueMember = "Key";
                comboBox1.DataSource = areas;

                AdjustComboBoxDropDownWidth(comboBox1);

                //Locations = await _service.FetchLocations();

                LoggedIn = true;

            }
            else
            {
                MessageBox.Show("Login failed.");
            }

        }

        private async void button2_Click(object sender, EventArgs e)
        {
            var schedule = await _service.FetchScheduleAsync(dtpDate.Value.ToString("M/d/yyyy"), (KeyValuePair<int, string>)comboBox1.SelectedItem);

            if (schedule.Count > 0)
            {
                //MessageBox.Show($"Fetched {schedule.Count} schedule entries.");

                
                ScheduleExporter.ExportToExcel(schedule, "", Locations.ToList());

            }
            else
            {
                MessageBox.Show("No schedule entries found.");
            }
        }

        void AdjustComboBoxDropDownWidth(ComboBox comboBox)
        {
            int maxWidth = comboBox.DropDownWidth;
            using (Graphics g = comboBox.CreateGraphics())
            {
                foreach (var item in comboBox.Items)
                {
                    string text = comboBox.GetItemText(item);
                    int width = (int)g.MeasureString(text, comboBox.Font).Width;
                    if (width > maxWidth)
                        maxWidth = width;
                }
            }

            comboBox.DropDownWidth = maxWidth + SystemInformation.VerticalScrollBarWidth;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var optionsForm = new LocationOptionsForm(_service, Locations);
            if (optionsForm.ShowDialog() == DialogResult.OK)
            {
                // Apply: replace Locations with updated list
                Locations = new BindingList<string>(optionsForm.UpdatedList.ToList());

                string exeDir = AppDomain.CurrentDomain.BaseDirectory;
                string filePath = Path.Combine(exeDir, _LocaitonsFileName);

                File.WriteAllLines(filePath, Locations);

            }
            else
            {
                // Cancel: do nothing
            }
        }

        private void UpdateUiState()
        {
            button2.Enabled = _loggedIn;
            button4.Enabled = _loggedIn;
            comboBox1.Enabled = _loggedIn;
            dtpDate.Enabled = _loggedIn;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            AboutForm about = new AboutForm();
            about.ShowDialog();
        }
    }


}
