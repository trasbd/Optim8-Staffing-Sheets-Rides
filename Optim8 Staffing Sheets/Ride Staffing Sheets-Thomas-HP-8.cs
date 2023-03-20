using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace Optim8_Staffing_Sheets
{
    public partial class Form1 : Form
    {
        public IWebDriver driver;
        public int sheetsMade = 0;
        //public string appDataFolder = Directory.GetCurrentDirectory();
        public string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Ride Staffing Sheets";
        public Form1()
        {
            InitializeComponent();
            
            
        }


        private void button1_Click(object sender, EventArgs e)
        {
            //Variables
            int areaNumber = cbArea.SelectedIndex+1;
            DateTime dateWanted = dtpDate.Value;
            double shiftStartTimeAlloance = .75; //In hours
            

            if (txtCompany.Text.Equals("") || txtID.Text.Equals("") || txtPass.Text.Equals(""))
            {
                lblError.Text = "Could not Login. Please check Username and Passowrd.";
            }
            else
            {

                try
                {
                    //if the driver isnt already open
                    if (driver == null)
                    {
                        ChromeOptions options = new ChromeOptions();

                        //Disables images so it loads faster
                        options.AddUserProfilePreference("profile.default_content_setting_values.images", 2);
                        driver = new ChromeDriver(options);
                        //makes form ontop of web browser
                        this.TopMost = true;
                        //Go to sixflags.team
                        driver.Navigate().GoToUrl("http://sixflags.team");
                        //finds login link
                        IWebElement loginLink = driver.FindElement(By.Id("alogin1"));
                        //clicks login link to display login block
                        loginLink.Click();

                        //Finds text boxes Company, User ID, and Password
                        IWebElement companyTxt = driver.FindElement(By.Id("txtCompany"));
                        IWebElement idTxt = driver.FindElement(By.Id("txtuserid"));
                        IWebElement passTxt = driver.FindElement(By.Id("txtpwd"));

                        //Sends the text entered in Form to text boxes on sixflags.team
                        companyTxt.SendKeys(txtCompany.Text);
                        idTxt.SendKeys(txtID.Text);
                        passTxt.SendKeys(txtPass.Text);

                        //Finds login button
                        IWebElement login = driver.FindElement(By.Id("btnlogin1"));
                        //click login button
                        login.Click();
                        //makes form no longer on top of everything
                        this.TopMost = false;
                        //if the url doesn't contain "/tm" then it didnt get redirected after logging in
                        //So something went wrong in the login process
                        //Most possible source of error is invalid username or password
                        if (!driver.Url.Contains("/tm"))
                        {

                            lblError.Text = "Could not Login. Please check Username and Passowrd.";
                            //closes browser
                            driver.Quit();
                            //sets driver to null
                            driver = null;
                        }
                    }

                    //After sucessful login

                    //Redirect Browser to the scheduling webpage
                    driver.Navigate().GoToUrl("http://sixflags.team/tm/tm/schedule");

                    //finds the department Drop Down element
                    SelectElement departmentDropDown = new SelectElement(driver.FindElement(By.Id("ddd2")));
                    //Sets the department to 'blank' because some areas have attractions and rides
                    departmentDropDown.SelectByIndex(0);

                    //Finds the Drop Down to select ride area
                    SelectElement areaDropDown = new SelectElement(driver.FindElement(By.Id("ddarea")));
                    //Sets the ride area to the number selected on form
                    areaDropDown.SelectByText("Rides Area " + areaNumber);

                    //Finds the date from box
                    IWebElement dateFrom = driver.FindElement(By.Id("txtFrom"));
                    //clears it so its ready to recieve a new date typed in
                    dateFrom.Clear();
                    //Sends dateWanted to date from box
                    dateFrom.SendKeys(dateWanted.ToShortDateString());
                    //Same as date from but its date to
                    IWebElement dateTo = driver.FindElement(By.Id("txtTo"));
                    dateTo.Clear();
                    dateTo.SendKeys(dateWanted.ToShortDateString());

                    //finds the go button
                    IWebElement goBtn = driver.FindElement(By.Id("divgo"));

                    //Finds the table where schedules will be displayed
                    //IWebElement scheduleTable = driver.FindElement(By.Id("tbgrid1"));
                    IWebElement scheduleTable = driver.FindElement(By.Id("divgrid0"));

                    //before new table is generated saves old table for comparison
                    String oldTable = scheduleTable.Text;

                    //Clicks Go to load new table
                    goBtn.Click();

                    //Grabs new table
                    String rawTable = scheduleTable.Text;

                    int looped = 0;
                    bool again = true;

                    while (again)
                    {
                        //If new table is the same as old table it pulls in the table again
                        if (rawTable.Equals(oldTable))
                            again = true;
                        else
                            again = false;
                        rawTable = scheduleTable.Text;
                        looped++;
                        //if the old table is the same as the new table after pulling it 100 times
                        //its probaly the table we want
                        if (looped > 100)
                            again = false;

                    }

                    ///*
                    //while (!rawTable.Contains(dateWanted.ToShortDateString()) || !rawTable.Contains("Department Location Position Seq. Time"))
                    //{
                    //    rawTable = scheduleTable.Text;
                    //    Console.WriteLine(rawTable);
                    //}
                    //


                    
                    //Removes unnessicary strings
                    rawTable = rawTable.Replace("Ride Operations ", "");
                    rawTable = rawTable.Replace("Arcade Att ", "");

                    //Writes table to file so it can be read from as a string
                    //File is stored in program directory
                    System.IO.File.WriteAllText(appDataFolder+"rawTable.dat", rawTable);

                    string line;
                    

                    //Reading table file
                    System.IO.StreamReader file = new System.IO.StreamReader(appDataFolder+"rawTable.dat");

                    //if table contains no records of actual schedules
                    if ((line = file.ReadLine()) != null && line.Contains("No record found."))
                    {
                        //Displays message box
                        MessageBox.Show("No schedules have been posted for Area " + areaNumber + " on " + dateWanted.ToShortDateString());
                        //Close file stream
                        file.Close();
                        //Deletes table file
                        File.Delete("rawTable.dat");
                    }
                    else
                    {
                        //*****************************************************************************
                        //**When referencing 'everyone' assume everyone within area and date selected**
                        //*****************************************************************************


                        //Making a list of everyone scheduled
                        var people = new List<individualSchedule>();
                        //reads until there are no more lines to read
                        while ((line = file.ReadLine()) != null)
                        {
                            //making a person from table
                            //see individualSchedule(string) constructor to see how person is built
                            var person = new individualSchedule(line);
                            //adds person to list
                            people.Add(person);
                        }

                        //after reading all the lines
                        //Closes file
                        file.Close();
                        //Deletes table file
                        File.Delete("rawTable.dat");


                        //[ride][shift][person]
                        //Making a list of rides
                        var area = new List<ride>();
                        //Making first ride the ride of the first person in list
                        area.Add(new ride(people.ElementAt(0).m_ride));
                        //adds the first person to their ride in a unsorted shift
                        area.ElementAt(0).m_shift[0].m_crew.Add(people.ElementAt(0));

                        //For the entire list of people starting at the second entry
                        //Creating the correct number of rides with ride names for everyone
                        for (int i = 1; i < people.Count(); i++)
                        {
                            
                            //if the ride name of current person is different from the previous person
                            if (!people.ElementAt(i).m_ride.Equals(people.ElementAt(i - 1).m_ride))
                            {
                                //This assumes the people scheduled are listed in order by A/C location
                                //adds a new ride to the list
                                area.Add(new ride(people.ElementAt(i).m_ride));
                            }
                            //adds person to their ride
                            //m_shift[0] is a null shift (not day, night, nor swing) for everyone scheduled at the ride
                            area.ElementAt(area.Count - 1).m_shift[0].m_crew.Add(people.ElementAt(i));

                        }

                        //*************************************************
                        //**SORTS PEOPLE INTO DAY, SWING, AND NIGHT SHIFT**
                        //*************************************************
                        
                        int count = 0;
                        //for each ride
                        foreach (var ride in area)
                        {
                            //then for each person in a ride 
                            foreach (var person in ride.m_shift[0].m_crew)
                            {
                                if (ride.m_shift[1].m_crew.Count == 0)
                                {
                                    ride.m_shift[1].m_crew.Add(person);
                                }
                                else
                                {
                                    //if neg shift start is earlier
                                    //if pos shift start is after
                                    //0 equal
                                    int compared = DateTime.Compare(ride.m_shift[1].m_crew.ElementAt(0).m_start, person.m_start);
                                    int comparedP1 = DateTime.Compare(ride.m_shift[1].m_crew.ElementAt(0).m_start.AddHours(shiftStartTimeAlloance), person.m_start);
                                    int comparedP2 = DateTime.Compare(ride.m_shift[1].m_crew.ElementAt(0).m_start.AddHours(-shiftStartTimeAlloance), person.m_start);
                                    if (compared == 0 || (comparedP1 > 0 && comparedP2 < 0))
                                    {
                                        ride.m_shift[1].m_crew.Add(person);
                                    }
                                    else
                                    {
                                        if (ride.m_shift[2].m_crew.Count == 0)
                                            ride.m_shift[2].m_crew.Add(person);
                                        else
                                        {
                                            compared = DateTime.Compare(ride.m_shift[2].m_crew.ElementAt(0).m_start, person.m_start);
                                            comparedP1 = DateTime.Compare(ride.m_shift[2].m_crew.ElementAt(0).m_start.AddHours(shiftStartTimeAlloance), person.m_start);
                                            comparedP2 = DateTime.Compare(ride.m_shift[2].m_crew.ElementAt(0).m_start.AddHours(-shiftStartTimeAlloance), person.m_start);
                                            if (compared == 0 || (comparedP1 > 0 && comparedP2 < 0))
                                            {
                                                ride.m_shift[2].m_crew.Add(person);
                                            }
                                            else
                                            {
                                                if (ride.m_shift[3].m_crew.Count == 0)
                                                    ride.m_shift[3].m_crew.Add(person);
                                                else
                                                {
                                                    compared = DateTime.Compare(ride.m_shift[3].m_crew.ElementAt(0).m_start, person.m_start);
                                                    comparedP1 = DateTime.Compare(ride.m_shift[3].m_crew.ElementAt(0).m_start.AddHours(shiftStartTimeAlloance), person.m_start);
                                                    comparedP2 = DateTime.Compare(ride.m_shift[3].m_crew.ElementAt(0).m_start.AddHours(-shiftStartTimeAlloance), person.m_start);
                                                    if (compared == 0 || (comparedP1 > 0 && comparedP2 < 0))
                                                    {
                                                        ride.m_shift[3].m_crew.Add(person);
                                                    }
                                                        //if they dont fit within 1 hour of the other list they are put here                                                    
                                                    else
                                                    {
                                                        ride.m_shift[4].m_crew.Add(person);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                count++;
                            }
                        }

                        //************************
                        //**REMOVING EMPTY RIDES**
                        //************************
                        //empty rides are sometimes created from junk in rawTable
                        List<int> rideToRemove = new List<int>();
                        for (int i = area.Count - 1; i >= 0; i--)
                        {
                            if (area.ElementAt(i).m_name.Equals(""))
                                rideToRemove.Add(i);
                        }
                        foreach (var index in rideToRemove)
                        {
                            area.RemoveAt(index);
                        }

                        //Counting list used in each ride
                        foreach (var ride in area)
                        {
                            for (int i = 1; i < 4; i++)
                            {
                                if (ride.m_shift[i].m_crew.Count > 0)
                                    ride.m_listUsed++;
                            }

                            //Seeing Which list is Day, Night, and Swing (there could be 2 swing list)
                            //counting how many shift starts are after current shift start
                            for (int i = 1; i <= ride.m_listUsed; i++)
                            {
                                int numLessThan = 0;
                                for (int j = 1; j <= ride.m_listUsed; j++)
                                {
                                    if (j != i)
                                    {
                                        if (DateTime.Compare(ride.m_shift[i].m_crew.ElementAt(0).m_start, ride.m_shift[j].m_crew.ElementAt(0).m_start) < 0)
                                        {
                                            numLessThan++;
                                        }
                                    }

                                }
                                //Assigns Shift Names

                                //If the start time is less than all of the other shifts then it has to be Day
                                //if the start time is after all the other shifts then the shift is Night
                                //else Swing
                                if (numLessThan == (ride.m_listUsed - 1))
                                {
                                    ride.m_shift[i].m_shiftTime = shiftTime.Day;
                                }
                                else if (numLessThan < (ride.m_listUsed - 1) && numLessThan > 0)
                                    ride.m_shift[i].m_shiftTime = shiftTime.Swing;
                                else
                                    ride.m_shift[i].m_shiftTime = shiftTime.Night;
                                //Console.WriteLine(ride.m_shift[i].m_shiftTime);
                            }
                            //Checking to see if there are multiple swing list
                            List<int> swingList = new List<int>();
                            for (int i = 1; i <= ride.m_listUsed; i++)
                            {
                                if (ride.m_shift[i].m_shiftTime == shiftTime.Swing)
                                {
                                    //Adding the shift list number to a list
                                    swingList.Add(i);
                                }
                            }
                            //if there are 2 swing shifts then they will be combined into 1
                            if (swingList.Count == 2)
                            {
                                //adds the higher index swing shift to the lower index to keep empty list towards the end
                                ride.m_shift[swingList.ElementAt(0)].m_crew.AddRange(ride.m_shift[swingList.ElementAt(1)].m_crew);
                                //Clear other swing shift
                                ride.m_shift[swingList.ElementAt(1)].m_crew.Clear();
                                //assigns to a null shift
                                ride.m_shift[swingList.ElementAt(1)].m_shiftTime = shiftTime.None;
                            }


                            //On '1 shift' days there will be a day and swing shift
                            //When sorting shifts the swing will look like night
                            //Here the night shift will be labeled swing if there '1 shift'
                            //Even if the swing is really a swing close it would still be prefered to look like a swing shift


                            int temp = 0;
                            //Counts number of list used
                            //eh probs could make this a member function (prob should)
                            for (int i = 1; i <= ride.m_listUsed; i++)
                            {

                                if (ride.m_shift[i].m_shiftTime != shiftTime.None)
                                {

                                    temp++;

                                }
                            }
                            //if there are only 2 shifts
                            if (temp == 2)
                            {
                                //uses m_listUsed because one of the list could be after a null list
                                for (int i = 1; i <= ride.m_listUsed; i++)
                                {
                                    if (ride.m_shift[i].m_shiftTime == shiftTime.Night)
                                        ride.m_shift[i].m_shiftTime = shiftTime.Swing;
                                }
                            }
                        }

                        //adds to the number of excel files made
                        sheetsMade++;
                        string fileName = "\\sheet" + sheetsMade + ".xls";

                        //creates appdata folder if not created
                        if (!Directory.Exists(appDataFolder))
                        {
                            Directory.CreateDirectory(appDataFolder);
                        }


                        //Prints list to excel
                        printToExcel(area, areaNumber, dateWanted, fileName);

                        //Opens up excel file
                        System.Diagnostics.Process proc = new System.Diagnostics.Process();
                        proc.StartInfo.FileName = appDataFolder +fileName;
                        proc.StartInfo.UseShellExecute = true;
                        proc.Start();


                    }
                }




                catch (OpenQA.Selenium.NoSuchElementException)
                {
                    MessageBox.Show("Could not login!\nPlease check Internet connection.");
                }
                catch (Exception ex)
                {
                    if (driver != null)
                    {
                        driver.Quit();
                        driver = null;
                    }
                    
                    if (ex.ToString().Contains("incorrect"))
                    {
                        lblError.Text = "Make sure your Username and Password are correct.";
                    }
                    else
                    {
                        MessageBox.Show("An Error has occured. Please try again.");
                    }
                }
            
    }

}


                
        //Creates a Staffing Sheet Excel spreadsheet for an Area for a certain Date passed
        //Pre: area really should contain at least 1 ride (will make blank staffing sheet if 0 rides)
        //Post: Creates an .xls file in the program directory
        private void printToExcel(List<ride> area, int areaNumber, DateTime dateWanted, string filename)
        {
            Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();

            //Checks if Excel is installed
            //User will have to install Excel themself
            if (xlApp == null)
            {
                MessageBox.Show("Excel is not installed!");
            }
            else
            {
                Excel.Workbook xlWorkBook;
                Excel.Worksheet xlWorkSheet;
                
                //From what I understand misValue is used similar to null/default
                object misValue = System.Reflection.Missing.Value;


                xlWorkBook = xlApp.Workbooks.Add(misValue);
                xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                //Adds Area Number to top left
                xlWorkSheet.Cells[1, 1] = "Area " + areaNumber;
                //Adds date to top right
                xlWorkSheet.Cells[1, 5] = dateWanted.ToShortDateString();

                //If previous date puts message saying the staffing sheet may not reflect the actual operators at the ride at that time
                if (DateTime.Compare(dateWanted.Date, DateTime.Now.Date)<0)
                {
                    xlWorkSheet.Cells[1, 3] = "*May not represent accurate staffing for previous days*";
                }

                int row = 3;

                //For each ride in the area
                foreach (var ride in area)
                {
                    //adds ride name to center, bold it, and Underline
                    xlWorkSheet.Cells[row, 3] = ride.m_name;
                    xlWorkSheet.Cells[row, 3].Font.Bold = true;
                    xlWorkSheet.Cells[row, 3].Font.Underline = true;
                    xlWorkSheet.Cells[row, 3].HorizontalAlignment = 3;

                    //skips down 2 rows
                    row+=2;
                    int max_row=0;
                    int start_row=row;
                    for (int i =1; i<4;i++)
                    {
                        start_row = row;
                        int col=0;
                        if (ride.m_shift[i].m_shiftTime==shiftTime.Day)
                            col=1;
                        else if (ride.m_shift[i].m_shiftTime == shiftTime.Swing)
                            col = 3;
                        else if (ride.m_shift[i].m_shiftTime == shiftTime.Night)
                            col =5;

                        foreach (var person in ride.m_shift[i].m_crew)
                        {
                            xlWorkSheet.Cells[start_row, col] = person.m_start.ToShortTimeString() + "-" + person.m_end.ToShortTimeString() + "  " + person.m_name;
                            string colC;
                            switch (col)
                            {
                                case(1):
                                    colC="a";
                                        break;
                                case(3):
                                        colC = "c";
                                        break;
                                case(5):
                                        colC = "e";
                                        break;
                                default:
                                        colC = "a";
                                    break;
                            }
                            xlWorkSheet.get_Range(colC+start_row, colC+start_row).Borders[Excel.XlBordersIndex.xlEdgeBottom].Color = System.Drawing.Color.Black;
                            start_row++;
                        }
                        if (start_row > max_row)
                            max_row = start_row;

                    }
                    row=max_row+1;
                }

                //Auto Sizes Cell Colums
                xlWorkSheet.Columns.AutoFit();

                //saves to program directory 
                xlWorkBook.SaveAs(appDataFolder+filename,Excel.XlFileFormat.xlWorkbookNormal,misValue,misValue,misValue,misValue,Excel.XlSaveAsAccessMode.xlExclusive,misValue,misValue,misValue,misValue,misValue);
                
                //Closes file correctly
                xlWorkBook.Close(true, misValue, misValue);
                xlApp.Quit();
                //Closes Excel objects correctly
                Marshal.ReleaseComObject(xlWorkSheet);
                Marshal.ReleaseComObject(xlWorkBook);
                Marshal.ReleaseComObject(xlApp);

 
            }
            //return is void
            return;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Sets area dropdown on form to area 1 onLoad
            cbArea.SelectedIndex = 0;
        }

        //Closes browser and deletes files
        //Pre: Excel files should be closed already
        //Post: Files are deleted and browser is closed
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //if the browser is still connected
            if (driver != null)
            {
                
                //quits the driver
                driver.Quit();
                //sets driver to null
                driver = null;
            }

            try
            {
                for (int i = sheetsMade; i > 0; i--)
                {
                    //Deletes all the sheets created
                    File.Delete(appDataFolder + "\\sheet" + i + ".xls");
                }
            }
            catch (System.IO.IOException)
            {
                //So if some excels file are open it will not delete all the files
                MessageBox.Show("Some Excel files are still open and could not be removed.");
                //When the program is used again it may ask if files want to be over written because of this
            }

        }
    }
}


