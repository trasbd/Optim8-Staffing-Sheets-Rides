using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace O8SS_WebRequest
{
    public class ScheduleService
    {
        private readonly HttpClient _client;

        private const string baseDomain = "optim8.com";

        private const string DEFAULT_DIVISION = "Operations";
        private const string DEFAULT_DEPARTMENT = "Ride Operations";

        public bool IsParkServices { get; set; }

        private string Department => IsParkServices ? "Park Services" : DEFAULT_DEPARTMENT;

        public ScheduleService(HttpClient client)
        {
            _client = client;
        }

        public async Task<bool> PerformLoginAsync(string company, string username, string password)
        {
            string loginUrl = $"https://{baseDomain}/tm/account/login";

            var loginData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("txtCompany", company),
                new KeyValuePair<string, string>("txtuserid", username),
                new KeyValuePair<string, string>("txtpwd", password)
            });

            try
            {
                var response = await _client.PostAsync(loginUrl, loginData);
                var content = await response.Content.ReadAsStringAsync();
                return response.IsSuccessStatusCode && !content.Contains("not correct");
            }
            catch
            {
                return false;
            }
        }

        // 1. Core HTTP/Parser abstraction
        private async Task<Dictionary<string, int>> FetchAndParseDropdownAsync(string url)
        {
            try
            {
                var response = await _client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var html = await response.Content.ReadAsStringAsync();

                return ScheduleParser.ParseDropDownToDictionary(html);
            }
            catch
            {
                return new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            }
        }



        // 2. Fetch Divisions
        public async Task<Dictionary<string, int>> FetchDivisionsAsync()
        {
            return await FetchAndParseDropdownAsync($"https://{baseDomain}/tm/commHandler/getdivision/?companyid=1");
        }

        // 3. Fetch Departments directly using a Division NAME
        public async Task<Dictionary<string, int>> FetchDepartmentsAsync(string divisionName = DEFAULT_DIVISION)
        {
            // Resolve the division ID right here first
            var divisions = await FetchDivisionsAsync();
            if (!divisions.TryGetValue(divisionName, out int divisionId))
            {
                // If the division name doesn't exist, we can't look up departments
                return new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            }

            return await FetchAndParseDropdownAsync($"https://{baseDomain}/tm/commHandler/getdept/?divisionid={divisionId}");
        }

        public async Task<int> GetDepartmentIdAsync(string departmentName)
        {
            var departments = await FetchDepartmentsAsync();

            if(departments.TryGetValue(departmentName, out int departmentId))
                    {  return departmentId; }
            return -1;
        }

        public async Task<List<KeyValuePair<int, string>>> FetchAreas()
        {
            string areaUrl = $"https://{baseDomain}/tm/commHandler/getarea/?companyid=1&_=1752356528300";

            try
            {
                var response = await _client.GetAsync(areaUrl); // ✅ Await this!
                response.EnsureSuccessStatusCode(); // ✅ Throws if not 200 OK
                var html = await response.Content.ReadAsStringAsync();
                return ScheduleParser.ParseAreas(html); // ✅ Not ScheduleParser
            }
            catch
            {
                return new List<KeyValuePair<int, string>>();
            }
        }

        public async Task<Dictionary<string, int>> FetchLocationsAsync(string department = null)
        {
            department = department ?? Department;
            Console.WriteLine("Fetching Locations");
            string areaUrl = $"https://{baseDomain}/tm/tm/schedule";

            try
            {
                // 1. Try the old way: Get the main schedule page HTML
                var response = await _client.GetAsync(areaUrl);
                response.EnsureSuccessStatusCode();

                var html = await response.Content.ReadAsStringAsync();
                var locations = ScheduleParser.ParseLocationOptions(html);

                // 2. Fallback check: If the original page didn't yield any locations, try the API
                if (locations == null || locations.Count == 0)
                {
                    Console.WriteLine("Fetching Locations Handler");
                    string locationApiUrl = $"https://{baseDomain}/tm/commHandler/getlocation/?deptid={await GetDepartmentIdAsync(department)}";

                    locations = await FetchAndParseDropdownAsync(locationApiUrl);
                }

                return locations;
            }
            catch
            {
                // Fallback if both attempts or network calls fail
                return new Dictionary<string, int>();
            }
        }

        public async Task<int> GetLocationIdAsync(string location)
        {
            var locations = await FetchLocationsAsync( Department  );

            if (locations.TryGetValue(location, out int id))
            { return id; }
            return -1;
        }

        public async Task<List<ScheduleEntry>> FetchScheduleAsync(string date, KeyValuePair<int, string> area, bool parkServicesOpt, IEnumerable<string> addlAC = null)
        {
            IsParkServices = parkServicesOpt;
            addlAC = addlAC ?? new List<string>();

            string scheduleUrl = $"https://{baseDomain}/tm/tm/schedulegrid/";

            var formData = CreateScheduleGridFormData(
                ddd2: (IsParkServices && area.Key == -1) ? "11" : "",
                txtFrom: date,
                ddarea: area.Key.ToString());

            try
            {
                string areaValue = area.Value;

                if (string.IsNullOrEmpty(areaValue))
                {
                    if (IsParkServices)
                    {
                        areaValue = "Park Services";
                    }
                    else
                    {
                        areaValue = "";
                    }
                }

                var response = await _client.PostAsync(scheduleUrl, formData);
                var html = await response.Content.ReadAsStringAsync();
                var schedules = ScheduleParser.ParseScheduleHtml(html, areaValue);

                
                    foreach (string loc in addlAC)
                    {
                        formData = CreateScheduleGridFormData(txtFrom: date, ddl1: (await GetLocationIdAsync(loc)).ToString());
                        response = await _client.PostAsync(scheduleUrl, formData);
                        html = await response.Content.ReadAsStringAsync();
                        schedules.AddRange(ScheduleParser.ParseScheduleHtml(html, areaValue));
                     }
                
                return schedules;
            }
            catch
            {
                return new List<ScheduleEntry>();
            }
        }

        public async Task<List<ScheduleEntry>> FetchRestroomScheduleAsync(List<ScheduleEntry> schedule)
        {
            string url = $"https://{baseDomain}/tm/hr/employeegrid/";

            FormUrlEncodedContent formData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("ddc1", "1"),
                new KeyValuePair<string, string>("ddd1", "3"),
                new KeyValuePair<string, string>("ddd2", "11"),
                new KeyValuePair<string, string>("txtempid", ""),
                new KeyValuePair<string, string>("txtpa", ""),
                new KeyValuePair<string, string>("txtsearchssn", ""),
                new KeyValuePair<string, string>("ddActive", "Active Only"),
                new KeyValuePair<string, string>("ddcert", ""),
                new KeyValuePair<string, string>("_gridA4B5AS45_FFB3B4_", "2"),
                new KeyValuePair<string, string>("_hidpagingtotal", "338"),
                new KeyValuePair<string, string>("_hidpagingcurrpage", "1"),
                new KeyValuePair<string, string>("_hidpagingmaxpage", "4"),
                new KeyValuePair<string, string>("txtpagecurrpage", "1"),
                new KeyValuePair<string, string>("txtpagesize", "1000"),
                new KeyValuePair<string, string>("_hiddentag", "grid")
            });

            try
            {
                HttpResponseMessage response = await _client.PostAsync(url, formData);
                response.EnsureSuccessStatusCode();
                string html = await response.Content.ReadAsStringAsync();

                Dictionary<int, string> homes = ScheduleParser.ParseHomeLocationHtml(html);

                if (homes.Count > 0)
                {
                    return ScheduleParser.CombineRestrooms(schedule, homes);
                }
            }
            catch
            {
                //Console.WriteLine($"Error fetching certifications: {ex.Message}");
                return schedule;
            }

            return schedule;
        }

        private FormUrlEncodedContent CreateScheduleGridFormData(
    string ddc1 = "1",
    string ddd1 = "",
    string ddd2 = "",
    string ddl1 = "",
    string txtFrom = null,
    string txtTo = null, 
    string ddarea = "", 
    string ddEmpFrom = "Company",
    string ddns = "",
    string ddview = "date",
    string ddfilter = "All Schedules",
    string ddvalidate = "all",
    string txtFrom2 = "7/7/2025",
    string txtTo2 = "7/13/2025",
    string ddschedulefrom = "Position",
    string txtemployeeid2 = "",
    string ddarea2 = "0",
    string chkactiveonly2 = "on",
    string txtdate3 = "7/7/2025",
    string txtdate32 = "7/7/2025",
    string ddcert3 = "",
    string txtdate4 = "7/12/2025",
    string ddlogtype = "",
    string txtemployeeid5 = "",
    string ddtype5 = "",
    string txtdate5 = "7/13/2025",
    string txtdate52 = "",
    string chksttobeprocessed = "1",
    string txtdate6 = "7/7/2025",
    string txtdate62 = "",
    string ddsbktime = "",
    string txtschedulerecoverdate = "",
    string txtschedulerecoverdate2 = "",
    string txtdate8 = "7/7/2025",
    string txtdate82 = "8/3/2025",
    string _gridA4B5AS45_FFB3B4_ = "5",
    string _hidpagingtotal = "0",
    string _hidpagingcurrpage = "0",
    string _hidpagingmaxpage = "",
    string _hiddentag = "grid",
    string act = "")
        {
            // Fallback logic for parameters that rely on runtime variables
            // (Assuming 'date', 'parkServices', and 'area' are accessible in this scope)
            txtFrom = txtFrom ?? DateTime.Now.ToString("d");
            txtTo = txtTo ?? txtFrom;
            

            var formData = new FormUrlEncodedContent(new[]
            {
        new KeyValuePair<string, string>("ddc1", ddc1),
        new KeyValuePair<string, string>("ddd1", ddd1),
        new KeyValuePair<string, string>("ddd2", ddd2),
        new KeyValuePair<string, string>("ddl1", ddl1),
        new KeyValuePair<string, string>("txtFrom", txtFrom),
        new KeyValuePair<string, string>("txtTo", txtTo),
        new KeyValuePair<string, string>("ddarea", ddarea),
        new KeyValuePair<string, string>("ddEmpFrom", ddEmpFrom),
        new KeyValuePair<string, string>("ddns", ddns),
        new KeyValuePair<string, string>("ddview", ddview),
        new KeyValuePair<string, string>("ddfilter", ddfilter),
        new KeyValuePair<string, string>("ddvalidate", ddvalidate),
        new KeyValuePair<string, string>("txtFrom2", txtFrom2),
        new KeyValuePair<string, string>("txtTo2", txtTo2),
        new KeyValuePair<string, string>("ddschedulefrom", ddschedulefrom),
        new KeyValuePair<string, string>("txtemployeeid2", txtemployeeid2),
        new KeyValuePair<string, string>("ddarea2", ddarea2),
        new KeyValuePair<string, string>("chkactiveonly2", chkactiveonly2),
        new KeyValuePair<string, string>("txtdate3", txtdate3),
        new KeyValuePair<string, string>("txtdate32", txtdate32),
        new KeyValuePair<string, string>("ddcert3", ddcert3),
        new KeyValuePair<string, string>("txtdate4", txtdate4),
        new KeyValuePair<string, string>("ddlogtype", ddlogtype),
        new KeyValuePair<string, string>("txtemployeeid5", txtemployeeid5),
        new KeyValuePair<string, string>("ddtype5", ddtype5),
        new KeyValuePair<string, string>("txtdate5", txtdate5),
        new KeyValuePair<string, string>("txtdate52", txtdate52),
        new KeyValuePair<string, string>("chksttobeprocessed", chksttobeprocessed),
        new KeyValuePair<string, string>("txtdate6", txtdate6),
        new KeyValuePair<string, string>("txtdate62", txtdate62),
        new KeyValuePair<string, string>("ddsbktime", ddsbktime),
        new KeyValuePair<string, string>("txtschedulerecoverdate", txtschedulerecoverdate),
        new KeyValuePair<string, string>("txtschedulerecoverdate2", txtschedulerecoverdate2),
        new KeyValuePair<string, string>("txtdate8", txtdate8),
        new KeyValuePair<string, string>("txtdate82", txtdate82),
        new KeyValuePair<string, string>("_gridA4B5AS45_FFB3B4_", _gridA4B5AS45_FFB3B4_),
        new KeyValuePair<string, string>("_hidpagingtotal", _hidpagingtotal),
        new KeyValuePair<string, string>("_hidpagingcurrpage", _hidpagingcurrpage),
        new KeyValuePair<string, string>("_hidpagingmaxpage", _hidpagingmaxpage),
        new KeyValuePair<string, string>("_hiddentag", _hiddentag),
        new KeyValuePair<string, string>("act", act)
    });

            return formData;
        }


    }
}
