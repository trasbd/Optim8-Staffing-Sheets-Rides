using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace O8SS_WebRequest
{
    public class ScheduleEntry
    {
        public int RowId { get; set; }
        public int DepartmentId { get; set; }
        public string Area { get; set; }
        public int LocationId { get; set; }
        public string DepartmentName { get; set; }
        public string LocationName { get; set; }
        public string PositionName { get; set; }
        public int Sequence { get; set; }
        public string TimeRange { get; set; }
        public string Date { get; set; }
        public int ScheduleId { get; set; }
        public EmployeeData EmployeeInfo { get; set; }

        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public int EID => EmployeeInfo.EID;
        public string Name => EmployeeInfo.Name;
        public AgeGroup Age => EmployeeInfo.Age;


        public override string ToString()
        {
            return LocationName + " (" + TimeRange + ")" + Name;
        }
    }

    public class EmployeeData
    {
        private string _info;

        public int EID { get; set; }
        public string Name { get; set; }
        public AgeGroup Age { get; set; }

        public EmployeeData(string info)
        {
            _info = info;
            ParseInfo(info);
        }

        private void ParseInfo(string info)
        {
            if (string.IsNullOrWhiteSpace(info))
                return;

            // Find first dash (between ID and Name)
            int dashIndex = info.IndexOf('-');
            if (dashIndex < 0)
                return;

            // Extract ID
            string idPart = info.Substring(0, dashIndex).Trim();
            if (int.TryParse(idPart, out int id))
                EID = id;

            // Extract name and optional age tag
            string nameAndTag = info.Substring(dashIndex + 1).Trim();

            int tagStart = nameAndTag.IndexOf('(');
            if (tagStart >= 0)
            {
                // Name before age tag
                Name = nameAndTag.Substring(0, tagStart).Trim();

                // Extract age tag
                string ageStr = nameAndTag.Substring(tagStart).Trim('(', ')').Trim();
                switch (ageStr)
                {
                    case "14-15":
                        Age = AgeGroup.YellowTag;
                        break;
                    case "16-17":
                        Age = AgeGroup.OrangeTag;
                        break;
                    default:
                        Age = AgeGroup.Unknown;
                        break;
                }
            }
            else
            {
                // No age tag
                Name = nameAndTag;
                Age = AgeGroup.WhiteTag;
            }
        }


    }

    public enum AgeGroup
    {
        Unknown,
        YellowTag, // (14-15)
        OrangeTag, // (16-17)
        WhiteTag
    }



}
