using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optim8_Staffing_Sheets
{
    enum shiftTime
    {
        None,
        Day,
        Night,
        Swing,
    }
    class shift
    {
        public shiftTime m_shiftTime;
        public List<individualSchedule> m_crew = new List<individualSchedule>();

    }
}
