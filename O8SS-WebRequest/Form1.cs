using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace O8SS_WebRequest
{
    public partial class Form1 : Form
    {
        private readonly ScheduleService _service;

        private const string LOCATION_FILE_NAME = "SortOrder.txt";
        private const string ADDL_AC_FILE_NAME = "AdditionalAC.txt";
        private BindingList<string> LocationsSortOrder = new BindingList<string>();
        private BindingList<string> AdditionalLocations = new BindingList<string>();

        private bool _updateSavedArea = false;

        private string[] _psLocations = { 
            "2600 - Park Services Clean Up",
            "2510 - PS Area 1",
            "2520 - PS Area 2",
            "2530 - PS Area 3",
            "2540 - PS Area 4",
            "2670 - Harbor Services",
            "2560 - PS Area E / Catering",
            "2570 - Women's Restrooms",
            "2580 - Men's Restrooms",
            "2800 - Park Services Supervisors",
            "2910 - Janitorial",
            "2550 - Washdown",
            "2590 - Waste/Trash Removal"
        };

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

            checkBoxRememberMe.Checked = LoadSettings();
            



            CookieContainer cookies = new CookieContainer();
            HttpClientHandler handler = new HttpClientHandler
            {
                CookieContainer = cookies,
                UseCookies = true,
                AllowAutoRedirect = true,
            };

            var client = new HttpClient(handler);
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
            client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
            client.Timeout = TimeSpan.FromSeconds(30);

            _service = new ScheduleService(client);


            string exeDir = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(exeDir, LOCATION_FILE_NAME);

            if (File.Exists(filePath))
            {
                LocationsSortOrder = new BindingList<string>(File.ReadAllLines(filePath).ToList());
            }

            filePath = Path.Combine(exeDir, ADDL_AC_FILE_NAME);
            if (File.Exists(filePath))
            {
                AdditionalLocations = new BindingList<string>(File.ReadAllLines(filePath).ToList());
            }


            if(checkBoxRememberMe.Checked)
            {
                _=AttemptLogin();
            }

        }

        private bool LoadSettings()
        {
            bool rememberMe = Properties.Settings.Default.SavedRemember;
            
            txtCompany.Text = string.IsNullOrEmpty(Properties.Settings.Default.SavedCompany) ? "epsl" : Properties.Settings.Default.SavedCompany;
            txtID.Text = Properties.Settings.Default.SavedUsername ?? "";

            string base64 = Properties.Settings.Default.SavedPassword;

            if (!string.IsNullOrWhiteSpace(base64) && rememberMe)
            {
                try
                {
                    byte[] encryptedBytes = Convert.FromBase64String(base64);
                    byte[] decryptedBytes = ProtectedData.Unprotect(encryptedBytes, null, DataProtectionScope.CurrentUser);
                    txtPass.Text = Encoding.UTF8.GetString(decryptedBytes);
                }
                catch (CryptographicException)
                {
                    // If the data was tampered with or invalid, clear it
                    txtPass.Text = "";
                    Properties.Settings.Default.SavedPassword = "";
                    Properties.Settings.Default.SavedRemember = false;
                    Properties.Settings.Default.Save();

                    return false;
                }
            }

            checkBoxPS.Checked = Properties.Settings.Default.SavedPS;
            NotesCheckBox.Checked = Properties.Settings.Default.SavedNotes;

            return rememberMe;
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

            

            await AttemptLogin();

            

        }

        private async Task AttemptLogin()
        {
            Enabled = false;
            if (await _service.PerformLoginAsync(txtCompany.Text, txtID.Text, txtPass.Text))
            {
                //MessageBox.Show("Login successful!");
                SaveSettings();

               _ = SortAreasAsync();

                LoggedIn = true;

            }
            else
            {
                MessageBox.Show("Login failed.");
            }

            Enabled = true;
        }

        private async Task<List<KeyValuePair<int, string>>> SortAreasAsync()
        {
            List<KeyValuePair<int, string>> areas;
            if (checkBoxPS.Checked)
            {
                areas = (await _service.FetchAreas())
                        .OrderBy(area =>
                        {
                            if (area.Value.StartsWith("PS "))
                                return 0;
                            else if (area.Value.Contains("Park Service"))
                                return 1;
                            else
                                return 2;
                        })
                        .ThenBy(area => area.Value) // alphabetical within groups
                        .ToList();
            }
            else
            {
                areas = (await _service.FetchAreas())
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
            }

            areas.Insert(0, new KeyValuePair<int, string>(-1, ""));

            _updateSavedArea = false;
            // Now populate the ComboBox
            AreaComboBox.DisplayMember = "Value";
            AreaComboBox.ValueMember = "Key";
            AreaComboBox.DataSource = areas;

            AreaComboBox.SelectedValue = Properties.Settings.Default.SavedAreaId;

            AdjustComboBoxDropDownWidth(AreaComboBox);

            _updateSavedArea = true;

            UpdateUiState();

            return areas;
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            Enabled = false;
            List<ScheduleEntry> schedule = await _service.FetchScheduleAsync(dtpDate.Value.ToString("M/d/yyyy"), (KeyValuePair<int, string>)AreaComboBox.SelectedItem, checkBoxPS.Checked, AdditionalLocations);

            if (schedule.Count > 0)
            {
                //MessageBox.Show($"Fetched {schedule.Count} schedule entries.");
                var locationOrder = LocationsSortOrder.ToList();

                if (checkBoxPS.Checked)
                {
                    locationOrder = _psLocations.ToList();
                    if(checkBoxRestrooms.Checked)
                        schedule = await _service.FetchRestroomScheduleAsync(schedule);
                }

                
                
                ScheduleExporter.ExportToExcel(schedule, "", locationOrder, NotesCheckBox.Checked, checkBoxPS.Checked);

            }
            else
            {
                MessageBox.Show("No schedule entries found.");
            }

            Enabled = true;
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

        private void UpdateUiState()
        {
            GoButton.Enabled = _loggedIn;
            SortOptionsButton.Enabled = _loggedIn && !checkBoxPS.Checked;
            AddlACOptionsButton.Enabled = _loggedIn && !checkBoxPS.Checked;
            AreaLabel.Enabled = AreaComboBox.Items.Count > 0;
            AreaComboBox.Enabled = AreaComboBox.Items.Count > 1;
            dtpDate.Enabled = _loggedIn;
            NotesCheckBox.Enabled = _loggedIn;
            checkBoxPS.Enabled = _loggedIn;
            checkBoxRestrooms.Enabled = _loggedIn && checkBoxPS.Checked;
            LoginButton.Enabled = !_loggedIn;
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            AboutForm about = new AboutForm();
            about.ShowDialog();
        }

        private void checkBoxRememberMe_CheckedChanged(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void SaveSettings()
        {
            if (checkBoxRememberMe.Checked)
            {
                Properties.Settings.Default.SavedCompany = txtCompany.Text;
                Properties.Settings.Default.SavedUsername = txtID.Text;
                byte[] encrypted = ProtectedData.Protect(
                    Encoding.UTF8.GetBytes(txtPass.Text),
                    null,
                    DataProtectionScope.CurrentUser);

                string encryptedBase64 = Convert.ToBase64String(encrypted);
                Properties.Settings.Default.SavedPassword = encryptedBase64;

            }
            else
            {
                Properties.Settings.Default.SavedCompany = "";
                Properties.Settings.Default.SavedUsername = "";
                Properties.Settings.Default.SavedPassword = "";

            }

            Properties.Settings.Default.SavedRemember = checkBoxRememberMe.Checked;
            Properties.Settings.Default.SavedPS = checkBoxPS.Checked;
            Properties.Settings.Default.SavedNotes = NotesCheckBox.Checked;

            Properties.Settings.Default.Save();

            
        }

        private void checkBoxPS_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxRestrooms.Visible = checkBoxPS.Checked;
            _service.IsParkServices = checkBoxPS.Checked;

            LoggedIn = LoggedIn;
            if (LoggedIn)
            {
                _ = SortAreasAsync();
                SaveSettings();
            }

            

            
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_updateSavedArea)
            {
                Properties.Settings.Default.SavedAreaId = (int)AreaComboBox.SelectedValue;
                Properties.Settings.Default.Save();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var optionsForm = new LocationOptionsForm(_service, LocationsSortOrder);
            if (optionsForm.ShowDialog() == DialogResult.OK)
            {
                // Apply: replace Locations with updated list
                LocationsSortOrder = new BindingList<string>(optionsForm.UpdatedList.ToList());

                string exeDir = AppDomain.CurrentDomain.BaseDirectory;
                string filePath = Path.Combine(exeDir, LOCATION_FILE_NAME);

                File.WriteAllLines(filePath, LocationsSortOrder);

            }
            else
            {
                // Cancel: do nothing
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var optionsForm = new AdditionalACOptionsForm(_service, AdditionalLocations);
            var ret = optionsForm.ShowDialog();
            if (ret == DialogResult.OK)
            {
                // Apply: replace Locations with updated list
                AdditionalLocations = new BindingList<string>(optionsForm.UpdatedList.ToList());

                string exeDir = AppDomain.CurrentDomain.BaseDirectory;
                string filePath = Path.Combine(exeDir, ADDL_AC_FILE_NAME);

                File.WriteAllLines(filePath, AdditionalLocations);

            }
            else
            {
                // Cancel: do nothing
            }
        }
    }


}
