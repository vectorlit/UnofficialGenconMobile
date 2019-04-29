using System;
using ConventionMobile;
using Xamarin.Forms;
using ConventionMobile.iOS;
using System.IO;
using UIKit;
using Foundation;
using EventKit;
using ConventionMobile.Model;
using System.Collections.Generic;
//using Plugin.Toasts;

[assembly: Dependency(typeof(Calendar_iOS))]
namespace ConventionMobile.iOS
{
    public class Calendar_iOS : ICalendar
    {
        public EKEventStore eventStore { get; set; } = null;
        //private bool accessGranted = false;

        #region ICalendar implementation
        public void CreateService()
        {
            //if (eventStore == null)
            //{
            //    eventStore = new EKEventStore();
            //    eventStore.RequestAccess(EKEntityType.Event,
            //        (bool granted, NSError e) =>
            //        {
            //            //if (granted)
            //            //{
            //            //    accessGranted = true;
            //            //}
            //            //else
            //            //{
            //            //    accessGranted = false;
            //            //}
            //        });
            //}
        }

        public Calendar_iOS()
        {
            CreateService();
        }

        void ICalendar.AddToCalendar(GenEvent genEvent)
        {
            var estZone = TimeZoneInfo.FindSystemTimeZoneById("America/Indiana/Indianapolis");
            var newStartTime = TimeZoneInfo.ConvertTime(genEvent.StartDateTime, estZone, TimeZoneInfo.Local);
            var newEndTime = TimeZoneInfo.ConvertTime(genEvent.EndDateTime, estZone, TimeZoneInfo.Local);

            eventStore = eventStore ?? new EKEventStore();

            eventStore.RequestAccess(EKEntityType.Event,
                (bool granted, NSError e) => {
                    if (granted)
                    {
                        EKEventStore eventStore = new EKEventStore();

                        //Insert the data into the agenda.
                        EKEvent newEvent = EKEvent.FromStore(eventStore);
                        newEvent.StartDate = CalendarHelpers.DateTimeToNSDate(newStartTime);
                        newEvent.EndDate = CalendarHelpers.DateTimeToNSDate(newEndTime);
                        newEvent.Title = genEvent.Title;
                        newEvent.Notes =
                            genEvent.Location + "\r\n\r\n"
                            + genEvent.Description + "\r\n\r\n"
                            + genEvent.LiveURL
                            ;

                        EKAlarm[] alarmsArray = new EKAlarm[1];
                        alarmsArray[0] = EKAlarm.FromDate(newEvent.StartDate.AddSeconds(-600));
                        newEvent.Alarms = alarmsArray;
                        
                        newEvent.Calendar = eventStore.DefaultCalendarForNewEvents;
                        eventStore.SaveEvent(newEvent, EKSpan.ThisEvent, true, out e);
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            //bool returned = await GlobalVars.notifier.Notify(ToastNotificationType.Success,
                            //    "Event added!", genEvent.Title + " has been added to your calendar!", TimeSpan.FromSeconds(3));
                            //var returned = await GlobalVars.notifier.Notify(new NotificationOptions()
                            //{
                            //    Title = "Event added!",
                            //    Description = genEvent.Title + " has been added to your calendar!",
                            //});
                            GlobalVars.DoToast(genEvent.Title + " has been added to your calendar!", GlobalVars.ToastType.Green);
                        });
                    }
                    //do something here
                    else
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            //bool returned = await GlobalVars.notifier.Notify(ToastNotificationType.Error,
                            //    "Don't have permission!", "You need to allow this app to access the calendar first. Please change the setting in Settings -> Privacy -> Calendars.", TimeSpan.FromSeconds(4));
                            //var returned = await GlobalVars.notifier.Notify(new NotificationOptions()
                            //{
                            //    Title = "Don't have permission!",
                            //    Description = "You need to allow this app to access the calendar first. Please change the setting in Settings -> Privacy -> Calendars.",
                            //});
                            GlobalVars.DoToast("Need permission - you need to allow this app to access the calendar first. Please change the setting in Settings -> Privacy -> Calendars.", GlobalVars.ToastType.Red, 5000);
                        });
                    }
                });
        }

        void ICalendar.CreateService()
        {
            //throw new NotImplementedException();
        }

        DateTime ICalendar.ConvertToIndy(DateTime dateTime)
        {
            var estZone = TimeZoneInfo.FindSystemTimeZoneById("America/Indiana/Indianapolis");
            return TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.Local, estZone);
        }
        #endregion

    }

    public static class CalendarHelpers
    {
        public static DateTime NSDateToDateTime(this NSDate date)
        {
            // NSDate has a wider range than DateTime, so clip
            // the converted date to DateTime.Min|MaxValue.
            double secs = date.SecondsSinceReferenceDate;
            if (secs < -63113904000)
                return DateTime.MinValue;
            if (secs > 252423993599)
                return DateTime.MaxValue;
            return (DateTime)date;
        }

        public static NSDate DateTimeToNSDate(this DateTime date)
        {
            if (date.Kind == DateTimeKind.Unspecified)
                date = DateTime.SpecifyKind(date, DateTimeKind.Local);
            return (NSDate)date;
        }
    }
}