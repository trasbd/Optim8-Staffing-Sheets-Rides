using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optim8_Staffing_Sheets
{
    class ride
    {
        public string m_name = "";
        public shift[] m_shift = new shift[5];
        public int m_listUsed = 0;

        public ride(string name)
        {
            for (int i = 0; i < 5; i++)
            {
                m_shift[i] = new shift();
            }
            m_name = name;
        }
    }
}
