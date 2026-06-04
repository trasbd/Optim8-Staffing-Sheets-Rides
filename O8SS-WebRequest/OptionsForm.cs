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
    public partial class OptionsForm : Form
    {
        protected readonly BindingList<string> _original;
        protected readonly BindingList<string> _editable;
        public BindingList<string> UpdatedList => _editable;

        protected readonly ScheduleService _scheduleService;

        public OptionsForm()
        {
            InitializeComponent();
        }

        public OptionsForm(ScheduleService service, BindingList<string> list)
        {
            InitializeComponent();
            _scheduleService = service;

            _original = list;
            _editable = new BindingList<string>(list.ToList()); // shallow clone

            listBox.DataSource = _editable;

            btnApply.DialogResult = DialogResult.OK;
            btnCancel.DialogResult = DialogResult.Cancel;
        }
    }
}
