using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace O8SS_WebRequest
{
    public class ScheduleExporter
    {
        public static void ExportToExcel(List<ScheduleEntry> entries, string filePath, List<string> locationOrder)
        {

            string Area = entries.FirstOrDefault()?.Area;
            string ScheduleDate = entries.FirstOrDefault()?.StartDateTime.ToString("M/d/yyyy");


            if (string.IsNullOrEmpty(filePath))
            {
                string cleanArea = new string(Area
                    .Where(c => !Path.GetInvalidFileNameChars().Contains(c) && !char.IsWhiteSpace(c))
                    .ToArray());

                string ScheduleDateNumbers = entries.FirstOrDefault()?.StartDateTime.ToString("yyyy-MM-dd");
                string filename = $"{ScheduleDateNumbers}_{cleanArea}_{DateTime.Now:yyyyMMdd-HHmmss}.xlsx";

                string subfolder = "GeneratedStaffingSheets"; // define the subfolder name
                string exeDir = AppDomain.CurrentDomain.BaseDirectory;
                string fullDir = Path.Combine(exeDir, subfolder);

                Directory.CreateDirectory(fullDir); // ensures folder exists

                filePath = Path.Combine(fullDir, filename);
            }


            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Schedule");

            ws.Cell(1, 1).Value = Area;
            ws.Cell(1, 5).Value = ScheduleDate;
            ws.Cell(1, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            int row = 3;

            Dictionary<string, int> locationIndexMap = locationOrder
                .Select((name, idx) => new { name, idx })
                .ToDictionary(x => x.name, x => x.idx);

            List<IGrouping<string, ScheduleEntry>> groupedEntries = entries
                .GroupBy(e => e.LocationName)
                .OrderBy(g => locationIndexMap.TryGetValue(g.Key, out int index) ? index : int.MaxValue)
                .ThenBy(g => g.Key) // fallback alphabetical
                .ToList(); // materialize so it's not re-evaluated

            foreach (IGrouping<string, ScheduleEntry> group in groupedEntries)
            {
                ws.Cell(row, 3).Value = group.Key;
                ws.Cell(row, 3).Style.Font.Bold = true;
                ws.Cell(row, 3).Style.Font.Underline = XLFontUnderlineValues.Single;
                ws.Cell(row, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                row++;
                row++;

                var list = group.OrderBy(e => e.StartDateTime).ToList();

                if (list.Count == 0)
                    continue;

                var earliest = list.Min(e => e.StartDateTime.TimeOfDay);
                var latest = list.Max(e => e.StartDateTime.TimeOfDay);

                var dayShift = list.Where(e =>
                    (e.StartDateTime.TimeOfDay - earliest).TotalMinutes <= 45 &&
                    (e.StartDateTime.TimeOfDay - earliest).TotalMinutes >= 0).ToList();

                var nightShift = list.Where(e =>
                    (latest - e.StartDateTime.TimeOfDay).TotalMinutes <= 45 &&
                    (latest - e.StartDateTime.TimeOfDay).TotalMinutes >= 0).ToList();

                var swingShift = list.Except(dayShift).Except(nightShift).ToList();

                int max = new[] { dayShift.Count, swingShift.Count, nightShift.Count }.Max();

                for (int i = 0; i < max; i++)
                {
                    if (i < dayShift.Count)
                    {
                        var cell = ws.Cell(row + i, 1);
                        var shift = dayShift[i];
                        cell.Value = $"{shift.StartDateTime:hh:mm tt}-{shift.EndDateTime:hh:mm tt} {shift.Name}";
                        cell.Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                        if (shift.Age == AgeGroup.YellowTag)
                            cell.Style.Fill.BackgroundColor = XLColor.Yellow;
                    }

                    if (i < swingShift.Count)
                    {
                        var cell = ws.Cell(row + i, 3);
                        var shift = swingShift[i];
                        cell.Value = $"{shift.StartDateTime:hh:mm tt}-{shift.EndDateTime:hh:mm tt} {shift.Name}";
                        cell.Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                        if (shift.Age == AgeGroup.YellowTag)
                            cell.Style.Fill.BackgroundColor = XLColor.Yellow;
                    }

                    if (i < nightShift.Count)
                    {
                        var cell = ws.Cell(row + i, 5);
                        var shift = nightShift[i];
                        cell.Value = $"{shift.StartDateTime:hh:mm tt}-{shift.EndDateTime:hh:mm tt} {shift.Name}";
                        cell.Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                        if (shift.Age == AgeGroup.YellowTag)
                            cell.Style.Fill.BackgroundColor = XLColor.Yellow;
                    }
                }


                row += max + 1;
            }

            ws.Column(1).AdjustToContents();
            ws.Column(3).AdjustToContents();
            ws.Column(5).AdjustToContents();

            wb.SaveAs(filePath);

            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = filePath,
                UseShellExecute = true // required to use default app
            });

        }


    }
}

