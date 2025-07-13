using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace O8SS_WebRequest
{
    public partial class LocationOptionsForm : Form
    {
        private readonly BindingList<string> _original;
        private readonly BindingList<string> _editable;
        public BindingList<string> UpdatedList => _editable;

        private readonly ScheduleService _scheduleService;


        public LocationOptionsForm(ScheduleService service, BindingList<string> locations)
        {
            InitializeComponent();

            _scheduleService = service;

            _original = locations;
            _editable = new BindingList<string>(locations.ToList()); // shallow clone

            listBox1.DataSource = _editable;

            btnApply.DialogResult = DialogResult.OK;
            btnCancel.DialogResult = DialogResult.Cancel;
        }

        private void listBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.listBox1.SelectedItem == null) return;
            this.listBox1.DoDragDrop(this.listBox1.SelectedItem, DragDropEffects.Move);
        }

        private void listBox1_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void listBox1_DragDrop(object sender, DragEventArgs e)
        {
            Point point = listBox1.PointToClient(new Point(e.X, e.Y));
            int index = this.listBox1.IndexFromPoint(point);
            if (index < 0) index = this.listBox1.Items.Count - 1;
            object data = e.Data.GetData(typeof(string));
            //this.listBox1.Items.Remove(data);
            _editable.Remove((string)data);
            //this.listBox1.Items.Insert(index, data);
            _editable.Insert(index, (string)data);
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            BindingList<string> newList = await _scheduleService.FetchLocations();

            _editable.Clear(); // Clear the current list to fire ListChanged events

            foreach (var item in newList)
                _editable.Add(item); // Triggers updates in bound UI like ListBox
        }
    }
}
