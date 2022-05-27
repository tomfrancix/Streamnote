using System;

namespace Streamnote.Relational.Helpers
{
    public class DateTimeHelper
    {
        public string GetFriendlyDateTime(DateTime datetime)
        {
            var now = DateTime.UtcNow;

            var newDate = "";

            var month = "";

            switch (datetime.Month)
            {
                case 1:
                    month = "Jan";
                    break;
                case 2:
                    month = "Feb";
                    break;
                case 3:
                    month = "March";
                    break;
                case 4:
                    month = "April";
                    break;
                case 5:
                    month = "May";
                    break;
                case 6:
                    month = "June";
                    break;
                case 7:
                    month = "July";
                    break;
                case 8:
                    month = "Aug";
                    break;
                case 9:
                    month = "Sep";
                    break;
                case 10:
                    month = "Oct";
                    break;
                case 11:
                    month = "Nov";
                    break;
                case 12:
                    month = "Dec";
                    break;
            }

            if (datetime.Year != now.Year)
            {
                newDate = datetime.Day + " " + month + " " + datetime.Year + " at " + datetime.ToShortTimeString();
            }
            else
            {
                newDate = datetime.Day + " " + month + " at " + datetime.ToShortTimeString();

                if (datetime.Month == now.Month)
                {
                    if (datetime.Day == now.Day)
                    {
                        var hours = now.Hour - datetime.Hour;

                        if (hours > 1)
                        {
                            newDate = hours + " hours ago";
                        } 
                        else if (hours > 0)
                        {
                            newDate = hours + " hour ago";
                        }
                        else
                        {
                            var minutes = now.Minute - datetime.Minute;
                            
                            if (minutes > 1)
                            {
                                newDate = minutes + " minutes ago";
                            }
                            else if (minutes > 0)
                            {
                                newDate = minutes + " minute ago";
                            }
                            else
                            {
                                var seconds = now.Second - datetime.Second;

                                if (seconds >= 10)
                                {
                                    newDate = seconds + " seconds ago";
                                }
                                else
                                {
                                    newDate = "just now";
                                }
                            }
                        }
                        
                    }
                }
            }

            return newDate;
        }
    }
}
