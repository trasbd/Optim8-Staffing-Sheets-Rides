using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optim8_Staffing_Sheets
{
    class individualSchedule
    {
        public string m_ride = "";
        public string m_position = "";
        public DateTime m_start;
        public DateTime m_end;
        public string m_name = "";
        public string m_Fname = "";


        public individualSchedule(string line)
        {
            if (!line.Contains("Total Hours") && !line.Contains("Department ") && char.IsDigit(line[0]))//Location Position Seq. Time"))
            {
                int index = 7;
                int lenght = 0;
                while (!isInt(line.Substring(index + lenght, 1)))
                {
                    lenght++;
                }
                //m_ride = line.Substring(index, lenght - 1);
                m_ride = line.Substring(0, index + lenght - 1);
                index += lenght;

                //Console.Write(m_ride);
                lenght = 0;
                index += 8;
                while (!isInt(line.Substring(index + lenght, 1)))
                {
                    lenght++;
                }
                m_position = line.Substring(index, lenght - 1);
                index += lenght;
                //Console.Write(m_position);

                index += 2;
                lenght = 0;
                if (line.Substring(index, 1).Equals(" "))
                    index++;

                String time = "";
                while (!line.Substring(index + lenght, 1).Equals("-"))
                {
                    lenght++;
                }
                time += line.Substring(index, lenght - 1);

                m_start = Convert.ToDateTime(time);

                //Console.Write(m_start.ToShortTimeString());

                index += lenght + 2;
                lenght = 0;
                time = "";

                while (!line.Substring(index + lenght, 1).Equals("M"))
                {
                    lenght++;
                }
                time += line.Substring(index, lenght + 1);

                m_end = Convert.ToDateTime(time);
                //Console.WriteLine(m_end.ToString());

                index += lenght + 2;
                lenght = 0;

                if (m_end < m_start)
                {
                    m_end.AddDays(1);
                }
                if (line.Length > index)
                {
                    //while (!line.Substring(index + lenght, 1).Equals(","))
                    //{
                    //    lenght++;
                    //}
                    //m_name= line.Substring(index, lenght);
                    //m_Fname = line.Substring(index + lenght + 1, line.Length);
                    
                    
                    index += 9;
                    m_name = line.Substring(index, line.Length - index);
                    //Console.Write(m_name);
                }


                //Console.WriteLine();
            }

        }

        //Determines if character passed is an integer
        //Pre: c should be a 1 character string
        //Post: returns true if c is an integer of 0-9 else false
        Boolean isInt(string c)
        {
            Boolean isItInt = false;
            for (int i = 0; i < 10; i++)
            {
                if (c.Equals(i.ToString()))
                {
                    isItInt = true;
                }
            }
            return isItInt;
        }

    }
}
