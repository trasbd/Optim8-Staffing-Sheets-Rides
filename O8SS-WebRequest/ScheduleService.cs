using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace O8SS_WebRequest
{
    public class ScheduleService
    {
        private readonly HttpClient _client;

        public ScheduleService(HttpClient client)
        {
            _client = client;
        }

        public async Task<bool> PerformLoginAsync(string company, string username, string password)
        {
            string loginUrl = "https://sixflags.team/tm/account/login";

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
                return response.IsSuccessStatusCode && !content.Contains("Invalid");
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<KeyValuePair<int, string>>> FetchAreas()
        {
            string areaUrl = "https://sixflags.team/tm/commHandler/getarea/?companyid=1&_=1752356528300";

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

        public async Task<BindingList<string>> FetchLocations()
        {
            string areaUrl = "https://sixflags.team/tm/tm/schedule";

            try
            {
                var response = await _client.GetAsync(areaUrl); // ✅ Await this!
                response.EnsureSuccessStatusCode(); // ✅ Throws if not 200 OK
                var html = await response.Content.ReadAsStringAsync();
                return ScheduleParser.ParseLocationOptions(html); // ✅ Not ScheduleParser
            }
            catch
            {
                return new BindingList<string>();
            }
        }


        public async Task<List<ScheduleEntry>> FetchScheduleAsync(string date, KeyValuePair<int, string> area)
        {
            string scheduleUrl = "https://sixflags.team/tm/tm/schedulegrid/";

            var formData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("ddc1", "1"),
                new KeyValuePair<string, string>("ddd1", ""),
                new KeyValuePair<string, string>("ddd2", ""),
                new KeyValuePair<string, string>("ddl1", ""),
                new KeyValuePair<string, string>("txtFrom", date),
                new KeyValuePair<string, string>("txtTo", date),
                new KeyValuePair<string, string>("ddarea", area.Key.ToString()),
                new KeyValuePair<string, string>("ddEmpFrom", "Company"),
                new KeyValuePair<string, string>("ddns", ""),
                new KeyValuePair<string, string>("ddview", "date"),
                new KeyValuePair<string, string>("ddfilter", "All Schedules"),
                new KeyValuePair<string, string>("ddvalidate", "all"),
                new KeyValuePair<string, string>("txtFrom2", "7/7/2025"),
                new KeyValuePair<string, string>("txtTo2", "7/13/2025"),
                new KeyValuePair<string, string>("ddschedulefrom", "Position"),
                new KeyValuePair<string, string>("txtemployeeid2", ""),
                new KeyValuePair<string, string>("ddarea2", "0"),
                new KeyValuePair<string, string>("chkactiveonly2", "on"),
                new KeyValuePair<string, string>("txtdate3", "7/7/2025"),
                new KeyValuePair<string, string>("txtdate32", "7/7/2025"),
                new KeyValuePair<string, string>("ddcert3", ""),
                new KeyValuePair<string, string>("txtdate4", "7/12/2025"),
                new KeyValuePair<string, string>("ddlogtype", ""),
                new KeyValuePair<string, string>("txtemployeeid5", ""),
                new KeyValuePair<string, string>("ddtype5", ""),
                new KeyValuePair<string, string>("txtdate5", "7/13/2025"),
                new KeyValuePair<string, string>("txtdate52", ""),
                new KeyValuePair<string, string>("chksttobeprocessed", "1"),
                new KeyValuePair<string, string>("txtdate6", "7/7/2025"),
                new KeyValuePair<string, string>("txtdate62", ""),
                new KeyValuePair<string, string>("ddsbktime", ""),
                new KeyValuePair<string, string>("txtschedulerecoverdate", ""),
                new KeyValuePair<string, string>("txtschedulerecoverdate2", ""),
                new KeyValuePair<string, string>("txtdate8", "7/7/2025"),
                new KeyValuePair<string, string>("txtdate82", "8/3/2025"),
                new KeyValuePair<string, string>("_gridA4B5AS45_FFB3B4_", "5"),
                new KeyValuePair<string, string>("_hidpagingtotal", "0"),
                new KeyValuePair<string, string>("_hidpagingcurrpage", "0"),
                new KeyValuePair<string, string>("_hidpagingmaxpage", ""),
                new KeyValuePair<string, string>("_hiddentag", "grid"),
                new KeyValuePair<string, string>("act", "")
            });

            try
            {
                var response = await _client.PostAsync(scheduleUrl, formData);
                var html = await response.Content.ReadAsStringAsync();
                return ScheduleParser.ParseScheduleHtml(html, area.Value);
            }
            catch
            {
                return new List<ScheduleEntry>();
            }
        }
    }
}
