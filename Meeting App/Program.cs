using System;
using System.Collections.Generic;
using System.IO;

namespace Meeting_App
{
    public class Meeting
    {
        private string title;
        private string location;
        private DateTime startDateTime;
        private DateTime endDateTime;

        public string Title
        {
            get
            {
                return this.title;
            }
            set
            {
                this.title = value;
            }
        }

        public string Location
        {
            get
            {
                return this.location;
            }
            set
            {
                this.location = value;
            }
        }

        public DateTime StartDateTime
        {
            get
            {
                return this.startDateTime;
            }
            set
            {
                this.startDateTime = value;
            }
        }

        public DateTime EndDateTime
        {
            get
            {
                return this.endDateTime;
            }
            set
            {
                this.endDateTime = value;
            }
        }
        public Meeting(string title, string location, DateTime startdatetime, DateTime enddatetime)
        {
            Title = title;
            Location = location;
            StartDateTime = startdatetime;
            EndDateTime = enddatetime;
        }
        public Meeting() { }

        public override string ToString()
        {
            return string.Format("Title: {0}\nLocation: {1}\nStart: {2}\nEnd: {3}",
                Title, Location, StartDateTime, EndDateTime);
        }
    }
    class Program
    {
        static void LoadCalendarFile()
        {
            List<Meeting> listOfMeetings = new List<Meeting>();
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string path = documentsPath + @"\calendar.dat";
            Console.Clear();

            try
            {
                IEnumerable<string> fileContents = File.ReadLines(path);

                foreach (string fileLine in fileContents)
                {
                    string[] fileLineData = fileLine.Split(",");
                    string name = fileLineData[0];
                    string location = fileLineData[1];
                    DateTime startTime = DateTime.Parse(fileLineData[2]);
                    DateTime endTime = DateTime.Parse(fileLineData[3]);

                    Meeting newMeeting = new Meeting(name, location, startTime, endTime);
                    listOfMeetings.Add(newMeeting);

                }
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("File not found.");
            }
            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine("Directory was not found.");
            }
            catch (PathTooLongException e)
            {
                Console.WriteLine("File Path was too long.");
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine("Access denied.");
            }
            Console.WriteLine("Calander has been loaded from:" + path);
            Console.WriteLine();

            CalendarMenu(listOfMeetings);
            
        }
        static void CreateNewCalendarMenu()
        {
            List<Meeting> listOfMeetings = new List<Meeting>();
            Console.Clear();
            Console.Write("Please Enter information for your first meeting:\nMeeting Name:");
            string firstMeetingName = StringEntryCheck();

            Console.Write("Location:");
            string firstMeetingLocation = StringEntryCheck();

            Console.Write("Start Date & Time:");
            DateTime firstMeetingStartTime = DateTimeEntryCheck(Console.ReadLine());

            Console.Write("End Date & Time:");
            DateTime firstMeetingEndTime = DateTimeEntryCheck(Console.ReadLine());

            Meeting firstMeeting = new Meeting(firstMeetingName, firstMeetingLocation, firstMeetingStartTime, firstMeetingEndTime);
            listOfMeetings.Add(firstMeeting);
            Console.Clear();

            CalendarMenu(listOfMeetings);

        }        
        static void CalendarMenu(List<Meeting> listOfMeetings)
        {
            int userChoice = -1;
            while (userChoice != 0)
            {
                Console.Write("What would you like to do?\n1)Add new meeting\n2)Remove existing meeting.\n3)View Schedule\n0)Save and Return to previous menu.\nChoice: ");
                string stringUserChoice = Console.ReadLine();

                try
                {
                    userChoice = Int32.Parse(stringUserChoice);
                }
                catch (Exception)
                {
                    //handled by default switch case
                }

                switch (userChoice)
                {
                    case 1:
                        {
                            listOfMeetings = AddNewMeeting(listOfMeetings);
                            break;
                        }
                    case 2:
                        {
                            listOfMeetings = RemoveExistingMeeting(listOfMeetings);
                            break;
                        }
                    case 3:
                        {
                            DisplayCalendar(listOfMeetings);
                            break;
                        }
                    case 0:
                        {
                            SaveCalendar(listOfMeetings);
                            break;
                        }
                    default:
                        {
                            Console.Clear();
                            Console.WriteLine("Invaild Entry! Try Again!\n");
                            break;
                        }
                }
            }
        }
        static List<Meeting> AddNewMeeting(List<Meeting> listOfMeetings)
        {
            Console.Clear();
            Console.Write("Meeting Name:");
            string NewMeetingName = Console.ReadLine();

            Console.Write("Location:");
            string newMeetingLocation = Console.ReadLine();

            Console.Write("Start Date & Time:");
            string stringNewMeetingStartTime = Console.ReadLine();
            DateTime newMeetingStartTime = DateTimeEntryCheck(stringNewMeetingStartTime);

            Console.Write("End Date & Time:");
            string stringNewMeetingEndTime = Console.ReadLine();
            DateTime newMeetingEndTime = DateTimeEntryCheck(stringNewMeetingEndTime);

            Meeting newMeeting = new Meeting(NewMeetingName, newMeetingLocation, newMeetingStartTime, newMeetingEndTime);
            listOfMeetings = ConflictCheck(newMeeting, listOfMeetings);

            Console.Clear();
            Console.WriteLine("Meeting added!\n");

            return listOfMeetings;
        }
        static string StringEntryCheck()
        {
            bool inputAccepted = false;
            string entry = null;
            do
            {
                try
                {
                    entry = Console.ReadLine();
                    inputAccepted = true;
                }
                catch (Exception)
                {
                    Console.Write("Invaild Entry. Try again: ");
                }
            } while (inputAccepted != true);

            return entry;
        }
        static DateTime DateTimeEntryCheck(string userEnteredTime)
        {
            DateTime meetingTime = new DateTime();
            bool inputAccepted = false;
            do
            {
                try
                {
                    meetingTime = DateTime.Parse(userEnteredTime);
                    if (meetingTime < DateTime.Now || meetingTime.Year > DateTime.Now.Year+1)
                    {
                        throw new FormatException();
                    }
                    else
                    {
                        inputAccepted = true;
                    }                                      
                }
                catch (Exception)
                {
                    Console.Write("Invaild Entry. Please try MM/DD/YYYY hhmm:");
                    userEnteredTime = Console.ReadLine();
                }
            } while (inputAccepted != true);
            return meetingTime;
        }
        static List<Meeting> ConflictCheck(Meeting newMeeting, List<Meeting> listOfMeetings)
        {
            List<Meeting> updatedListOfMeetings = listOfMeetings;

            foreach (Meeting i in listOfMeetings)
            {
                if ((newMeeting.StartDateTime > i.StartDateTime && newMeeting.StartDateTime < i.EndDateTime) || (newMeeting.EndDateTime > i.StartDateTime && newMeeting.EndDateTime < i.EndDateTime))
                {
                    Console.Clear();
                    Console.WriteLine("Warning: Schedule Conflict!\nThe new meeting overlaps with the following previously scheduled meeting:\nTitle:{0}\nLocation:{1}\nStart:{2}\nEnd:{3}",
                        i.Title, i.Location, i.StartDateTime, i.EndDateTime);
                    Console.WriteLine("Would you like to add the new meeting anyway? Y/N");
                    char choice = Convert.ToChar(Console.ReadLine());
                    if (choice == 'y' || choice == 'Y')
                    {
                        updatedListOfMeetings.Add(newMeeting);
                        break;
                    }
                }
                else
                {
                    updatedListOfMeetings.Add(newMeeting);
                    break;
                }
            }
            return updatedListOfMeetings;
        }        
        static List<Meeting> RemoveExistingMeeting(List<Meeting> listOfMeetings)
        {
            List<Meeting> updatedListOfMeetings = listOfMeetings;
            DisplayCalendar(listOfMeetings);

            Console.Write("Please Enter the name of the meeting you'd like to remove: ");
            string meetingToRemove = StringEntryCheck();
            bool meetingFound = false;
            foreach (Meeting i in listOfMeetings)
            {
                if (meetingToRemove.ToLower() == i.Title.ToLower())
                {
                    Console.WriteLine("Meeting removed.\n");
                    updatedListOfMeetings.Remove(i);
                    meetingFound = true;
                    break;
                }                
            }
            if (meetingFound == false)
            {
                Console.Clear();
                Console.WriteLine("No meetings with that name.");
            }
            return updatedListOfMeetings;
        }
        /*
        static void DisplayCalendar(List<Meeting> listOfMeetings)
        {
            Console.Clear();
            DateTime today = DateTime.Today.Date;
            string todayString = today.ToString("MM/dd");
            Console.WriteLine("Here's your next 14 days:\n");
            for (int i = 1; i <= 14; i++)
            {
                Console.Write(today.ToString("ddd") + " " + todayString + ":");
                foreach (Meeting meeting in listOfMeetings)
                {
                    if (meeting.StartDateTime.Date.ToString("MM/dd") == todayString)
                    {
                        Console.WriteLine();
                        Console.Write("\t\tTitle:" + meeting.Title);
                        Console.Write("\tLocation:" + meeting.Location);
                        Console.WriteLine();
                        Console.Write("\t\tStart:" + meeting.StartDateTime.ToString("t"));
                        Console.Write("\tEnd:" + meeting.EndDateTime.ToString("t"));
                    }
                }
                Console.WriteLine();
                today = today.AddDays(1);
                todayString = today.ToString("MM/dd");
            }
            Console.WriteLine();
        }
        */
        static void DisplayCalendar(List<Meeting> listOfMeetings)
        {
            Console.Clear();
            int numOfMeetings;
            DateTime day = DateTime.Today.Date;
            string shortDateString = day.ToString("MM/dd");
            Console.WriteLine("Here's your next 10 work days:\n");            
            for (int i = 1; i  <= 10; i++)
            {
                numOfMeetings = 0;
                if (day.DayOfWeek == DayOfWeek.Sunday || day.DayOfWeek == DayOfWeek.Saturday)
                {
                    i--;
                }
                else
                {
                    Console.Write(day.ToString("ddd") + " " + shortDateString + "| ");
                    foreach (Meeting meeting in listOfMeetings)
                    {
                        if (meeting.StartDateTime.Date.ToString("MM/dd") == shortDateString)
                        {
                            numOfMeetings++;
                        }
                    }
                    if (numOfMeetings > 0)
                    {
                        Console.Write(numOfMeetings + " meetings");
                    }
                    else
                    {
                        Console.Write("No meetings.");
                    }
                    Console.WriteLine();
                }
                day = day.AddDays(1);
                shortDateString = day.ToString("MM/dd");
            }
            Console.WriteLine();
            Console.Write("Which day would you like to view?\nChoice (MM/DD): ");
            DateTime dayToView = DateTimeEntryCheck(Console.ReadLine());

            Console.Clear();

            DateTime workDay = dayToView.AddHours(8);
            Console.WriteLine("Schedule for the day:\n");
            for (int i = 0; i <= 18; i++)
            {
                Console.Write(workDay.ToString("hh:mm:tt")+" | ");
                foreach (Meeting meeting in listOfMeetings)
                {
                    if (meeting.StartDateTime.ToString("MM/dd") == dayToView.ToString("MM/dd"))
                    {
                        if (meeting.StartDateTime <= workDay && meeting.EndDateTime >= workDay)
                        {
                            Console.Write(meeting.Title);
                        }
                    }
                }
                workDay = workDay.AddMinutes(30);
                Console.WriteLine();
            }
            Console.WriteLine();
        }
        static void SaveCalendar(List<Meeting> listOfMeetings)
        {
            Console.Clear();
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string path = documentsPath + @"\calendar.dat";
            try
            {
                    TextWriter tw = new StreamWriter(path);
                    foreach (Meeting i in listOfMeetings)
                    {
                        tw.WriteLine(i.Title + "," + i.Location + "," + i.StartDateTime + "," + i.EndDateTime);
                    }
                    tw.Close();
                }
                catch (FileNotFoundException e)
                {
                    Console.WriteLine("File not found.");
                }
                catch (DirectoryNotFoundException e)
                {
                    Console.WriteLine("Directory was not found.");
                }
                catch (PathTooLongException e)
                {
                    Console.WriteLine("File Path was too long.");
                }
                catch (UnauthorizedAccessException e)
                {
                    Console.WriteLine("Access denied.");
                }
            Console.WriteLine("Calendar has been saved to: "+documentsPath+"\\ as: calendar.dat\n");

        }    
        static void QuitApp()
        {
            Console.Clear();
            Console.WriteLine("Thanks for using the calendar app. Bye!");
            Environment.Exit(0);
        }
        static void DisplayMenu()
        {
            int choice;
            do
            {
                Console.WriteLine("Welcome to Meeting App!\nWhat would you like to do?\n1)Create new calendar\n2)Load calendar from file\n0)Exit");
                Console.Write("Selection: ");
                string schoice = Console.ReadLine();
                choice = -1;
                try
                {
                    choice = Int32.Parse(schoice);
                }
                catch
                {
                    //The default switch case handles the response for any invaild input.
                }
                switch (choice)
                {
                    case 1:
                        {
                            CreateNewCalendarMenu();
                            break;
                        }
                    case 2:
                        {
                            LoadCalendarFile();
                            break;
                        }
                    case 0:
                        {                            
                            QuitApp();
                            break;
                        }
                    default:
                        {
                            Console.WriteLine("Invaild Entry! Try again.\n");
                            break;
                        }
                }
            } while (choice != 0);
        }
        static void Main(string[] args)
        {
            //Write an application that lets you create a calendar of meetings. This application should let you add and remove meetings, as well as tell you about conflicting meetings (e.g. meetings that occur at the same time).
            //It should let you have multiple calendars that you can choose between (e.g. a Work calendar and a Personal calendar). It should also persist the calendars so that you can view them later.
            DisplayMenu();

            //Made by Tyler Santi
        }
    }
}
