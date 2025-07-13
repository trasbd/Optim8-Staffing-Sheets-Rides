using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;

namespace O8SS_WebRequest
{
    public static class ScheduleParser
    {
        public static List<ScheduleEntry> ParseScheduleHtml(string html, string area)
        {
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(WebUtility.HtmlDecode(html));

            var rows = doc.DocumentNode.SelectNodes("//tr[@rowid]") ?? Enumerable.Empty<HtmlNode>();

            var list = new List<ScheduleEntry>();

         


            foreach (var row in rows)
            {
                var cells = row.SelectNodes(".//td");
                if (cells == null)
                    continue;

                var chunks = cells
                    .Select((cell, index) => new { cell, index })
                    .GroupBy(x => x.index / 15); // 15 fields per schedule

                foreach (var chunk in chunks)
                {
                    var map = chunk.ToDictionary(
                        x => x.cell.GetAttributeValue("fldName", ""),
                        x => x.cell.InnerText.Trim());

                    if (!map.ContainsKey("id") || string.IsNullOrEmpty(map["id"]))
                        continue; // Skip junk rows (like "Total Hours")

                    var entry = new ScheduleEntry
                    {
                        RowId = SafeParseInt(map.ContainsKey("id") ? map["id"] : ""),
                        DepartmentId = SafeParseInt(map.ContainsKey("deptid") ? map["deptid"] : ""),
                        Area = area,
                        LocationId = SafeParseInt(map.ContainsKey("locationid") ? map["locationid"] : ""),
                        DepartmentName = map.ContainsKey("deptname") ? WebUtility.HtmlDecode(map["deptname"]) : "",
                        LocationName = map.ContainsKey("locationname") ? WebUtility.HtmlDecode(map["locationname"]) : "",
                        PositionName = map.ContainsKey("positionname") ? WebUtility.HtmlDecode(map["positionname"]) : "",
                        Sequence = SafeParseInt(map.ContainsKey("sequence") ? map["sequence"] : ""),
                        TimeRange = map.ContainsKey("time") ? WebUtility.HtmlDecode(map["time"]) : "",
                        EmployeeInfo = new EmployeeData(map.ContainsKey("date1") ? WebUtility.HtmlDecode(map["date1"]) : "")
                    };


                    var dateCell = chunk.FirstOrDefault(x => x.cell.GetAttributeValue("fldName", "") == "date1")?.cell;
                    entry.Date = dateCell?.GetAttributeValue("date", "") ?? "";

                    if (DateTime.TryParse(entry.Date, out var baseDate))
                    {
                        var timeParts = entry.TimeRange.Split('-');
                        if (timeParts.Length == 2)
                        {
                            var startTimeStr = timeParts[0].Trim();
                            var endTimeStr = timeParts[1].Trim();

                            if (DateTime.TryParse($"{entry.Date} {startTimeStr}", out var startDateTime) &&
                                DateTime.TryParse($"{entry.Date} {endTimeStr}", out var endDateTime))
                            {
                                // Handle end time being after midnight
                                if (endDateTime < startDateTime)
                                {
                                    endDateTime = endDateTime.AddDays(1);
                                }

                                entry.StartDateTime = startDateTime;
                                entry.EndDateTime = endDateTime;
                            }
                        }
                    }

                    list.Add(entry);
                }
            }

            return list;
        }



        public static List<KeyValuePair<int, string>> ParseAreas(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var result = new List<KeyValuePair<int, string>>();

            foreach (var option in doc.DocumentNode.SelectNodes("//option"))
            {
                var value = option.GetAttributeValue("value", "");
                var name = option.InnerText.Trim();

                if (int.TryParse(value, out int id) && !string.IsNullOrWhiteSpace(name))
                {
                    result.Add(new KeyValuePair<int, string>(id, name));
                }
            }

            return result;
        }

        public static BindingList<string> ParseLocationOptions(string html)
        { 
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            var options = doc.DocumentNode
                .SelectNodes("//select[@id='ddl1']/option")
                ?.Where(opt => !string.IsNullOrWhiteSpace(opt.InnerText))
                .Select(opt => WebUtility.HtmlDecode(opt.InnerText.Trim()))
                .ToList();

            return new BindingList<string>(options) ?? new BindingList<string>();
        }


        

        public static Dictionary<int, string> ParseHomeLocationHtml(string html)
        {
            var result = new Dictionary<int, string>();
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            // Look for table rows with 'rowid' attribute (actual employee rows)
            var rows = doc.DocumentNode.SelectNodes("//tr[@rowid]");
            if (rows == null) return result;

            foreach (var row in rows)
            {
                var cells = row.SelectNodes("td");
                if (cells == null || cells.Count < 7)
                    continue;

                string empNumber = cells[1].InnerText.Trim();   // column 2 = Employee Number
                string location = WebUtility.HtmlDecode(cells[6].InnerText.Trim()); // column 7 = Location

                if (!string.IsNullOrWhiteSpace(empNumber) && !string.IsNullOrWhiteSpace(location))
                {
                    result.Add(SafeParseInt(empNumber), location);
                }
            }

            return result;
        }


        public static List<ScheduleEntry> CombineRestrooms(List<ScheduleEntry> schedule, Dictionary<int, string> homes)
        {
            
            foreach (var restroomSchedule in schedule.FindAll(x => x.LocationName.Contains("Restroom")))
            {
                restroomSchedule.Restroom = true;
                if (homes.TryGetValue(restroomSchedule.EID, out string newLocation))
                {
                    restroomSchedule.LocationName = newLocation;
                }
            }

            return schedule;
        }

        private static int SafeParseInt(string input)
        {
            if (int.TryParse(input, out int result))
                return result;
            return 0;
        }
    }
}
