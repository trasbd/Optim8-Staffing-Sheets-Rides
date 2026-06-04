using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace O8SS_WebRequest
{
    public partial class AdditionalACOptionsForm : OptionsForm
    {

        private BindingList<string> _allLocations;

        public AdditionalACOptionsForm(ScheduleService service, BindingList<string> locations) : base(service, locations)
        {
            InitializeComponent();

            label1.Text = "Select additional locations to include.\nThese locations will be automatically added to the staffing sheet regardless of the selected area.";
        }

        private async void AdditionalACOptionsForm_LoadAsync(object sender, EventArgs e)
        {
            this.Enabled = false;
            try
            {
                // Correctly bind listBox1 to its own independent BindingList
                _allLocations = new BindingList<string>(await GetLocationsAsync());

                listBox1.DataSource = _allLocations;
            }
            finally
            {
                this.Enabled = true;
            }
        }

        public async Task<List<string>> GetLocationsAsync()
        {
            Dictionary<string, int> locations = await _scheduleService.FetchLocationsAsync();
            
            foreach(var item in _editable)
            {
                if (locations.ContainsKey(item))
                    locations.Remove(item);
            }

            return locations.Keys.ToList();
        }

        // --- ARROW NAVIGATION LOGIC ---

        private void btnRight_Click(object sender, EventArgs e)
        {
            MoveSelectedItems(listBox1, listBox);
        }

        private void btnLeft_Click(object sender, EventArgs e)
        {
            MoveSelectedItems(listBox, listBox1);
        }

        private void MoveSelectedItems(ListBox source, ListBox destination)
        {
            if (source.DataSource is BindingList<string> sourceList &&
                destination.DataSource is BindingList<string> destList)
            {
                // Copy selected items to an array first to avoid "Collection Modified" exceptions during enumeration
                var selectedItems = source.SelectedItems.Cast<string>().ToArray();
                source.ClearSelected();
                foreach (var itemStr in selectedItems)
                {
                    destList.Add(itemStr);
                    sourceList.Remove(itemStr);
                }
            }
        }

       
    }
}