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

                // Map by fldName directly. If a duplicate fldName exists, 
                // this takes the first one to avoid Dictionary insertion crashes.
                var cellMap = cells
                    .GroupBy(c => c.GetAttributeValue("fldName", ""))
                    .Where(g => !string.IsNullOrEmpty(g.Key))
                    .ToDictionary(g => g.Key, g => g.First());

                // Verify the ID column exists and isn't empty (filters out "Total Hours" rows)
                if (!cellMap.TryGetValue("id", out var idCell) || string.IsNullOrWhiteSpace(idCell.InnerText))
                    continue;

                // Helper lambda to safely extract and decode text from the dictionary map
                string GetInnerText(string key) =>
                    cellMap.TryGetValue(key, out var cell) ? WebUtility.HtmlDecode(cell.InnerText.Trim()) : "";

                // Safely extract the date1 cell reference for attribute harvesting
                cellMap.TryGetValue("date1", out var dateCell);

                var entry = new ScheduleEntry
                {
                    RowId = SafeParseInt(GetInnerText("id")),
                    DepartmentId = SafeParseInt(GetInnerText("deptid")),
                    Area = area,
                    LocationId = SafeParseInt(GetInnerText("locationid")),
                    DepartmentName = GetInnerText("deptname"),
                    LocationName = GetInnerText("locationname"),
                    PositionName = GetInnerText("positionname"),
                    Sequence = SafeParseInt(GetInnerText("sequence")),
                    TimeRange = GetInnerText("time"),
                    Date = dateCell?.GetAttributeValue("date", "") ?? "",
                    Note = dateCell != null ? WebUtility.HtmlDecode(dateCell.GetAttributeValue("note", "")) : "",
                    EmployeeInfo = new EmployeeData(GetInnerText("date1"))
                };

                // --- TIME PARSING LOGIC ---
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
                            // Handle overnight shifts (e.g., 10:15 PM - 3:00 AM)
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
        public static Dictionary<string, int> ParseLocationOptions(string html)
        {
            // Simply pass the specific dropdown ID "ddl1" into your combined parser
            return ParseDropDownToDictionary(html, dropdownId: "ddl1");
        }

        public static Dictionary<string, int> ParseDropDownToDictionary(string html, string dropdownId = null)
        {
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            var result = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            // 1. Decide how to grab the options: 
            //    If a specific dropdown ID is requested, look inside that select element.
            //    Otherwise, default to searching for any loose <option> elements (like API responses).
            string xpath = string.IsNullOrEmpty(dropdownId)
                ? "//option"
                : $"//select[@id='{dropdownId}']/option";

            var options = doc.DocumentNode.SelectNodes(xpath);
            if (options == null) return result;

            // 2. Loop through and build the dictionary safely
            foreach (var option in options)
            {
                var value = option.GetAttributeValue("value", "").Trim();
                var name = System.Net.WebUtility.HtmlDecode(option.InnerText.Trim());

                if (!string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(name))
                {
                    if (!result.ContainsKey(name))
                    {
                        if (int.TryParse(value, out int id))
                        {
                            result.Add(name, id);
                        }
                    }
                }
            }

            return result;
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
