using System;
using ConventionMobile;
using Xamarin.Forms;
using ConventionMobile.Droid;
using System.IO;
using SQLite;
using ConventionMobile.Model;
using Android.Content;
using Android.Provider;
using Java.Util;
using Plugin.CurrentActivity;

[assembly: Dependency(typeof(Calendar_Android))]

namespace ConventionMobile.Droid
{
    public class Calendar_Android : ICalendar
    {
        Context CurrentContext => CrossCurrentActivity.Current.Activity;

        public Calendar_Android()
        {

        }

        DateTime ICalendar.ConvertToIndy(DateTime dateTime)
        {
            var estZone = TimeZoneInfo.FindSystemTimeZoneById("America/Indianapolis");
            return TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.Local, estZone);
        }

        void ICalendar.AddToCalendar(GenEvent genEvent)
        {
            var estZone = TimeZoneInfo.FindSystemTimeZoneById("America/Indianapolis");
            var newStartTime = TimeZoneInfo.ConvertTime(genEvent.StartDateTime, estZone, TimeZoneInfo.Local);
            var newEndTime = TimeZoneInfo.ConvertTime(genEvent.EndDateTime, estZone, TimeZoneInfo.Local);

            //var newStartTime = genEvent.StartDateTime;
            //var newEndTime = genEvent.EndDateTime;

            var minTimeDifference = new TimeSpan(0, 15, 0);
            var actualTimeDifference = genEvent.EndDateTime.Subtract(genEvent.StartDateTime);

            if (actualTimeDifference < minTimeDifference)
            {
                newEndTime = TimeZoneInfo.ConvertTime(genEvent.StartDateTime.Add(minTimeDifference), estZone, TimeZoneInfo.Local);
                //newEndTime = genEvent.StartDateTime.Add(minTimeDifference);
            }

            var intent = new Intent(Intent.ActionInsert, Android.Net.Uri.Parse("content://com.android.calendar/events"))
                //.SetType("vnd.android.cursor.dir/event")
                .PutExtra(CalendarContract.ExtraEventBeginTime, GetDateTimeMS(newStartTime))
                .PutExtra(CalendarContract.ExtraEventEndTime, GetDateTimeMS(newEndTime))
                .PutExtra(CalendarContract.ExtraEventAllDay, false)
                //.PutExtra(CalendarContract.Events.InterfaceConsts.EventTimezone, "America/Indianapolis")
                //.PutExtra(CalendarContract.Events.InterfaceConsts.EventEndTimezone, "America/Indianapolis")
                .PutExtra(CalendarContract.Events.InterfaceConsts.Title, genEvent.Title)
                .PutExtra(CalendarContract.Events.InterfaceConsts.Description, genEvent.Location + "\r\n\r\n"
                            + genEvent.Description + "\r\n\r\n"
                            + genEvent.LiveURL)
                .PutExtra(CalendarContract.Events.InterfaceConsts.EventLocation, genEvent.Location)
                //.PutExtra(CalendarContract.Events.InterfaceConsts.Availability, EventsAvailability.Busy)
                //.PutExtra(CalendarContract.Events.InterfaceConsts.AccessLevel, EventsAccess.Private)
                ;

            //Forms.Context.StartActivity(Intent.CreateChooser(intent, "Create Calendar Event"));
            CurrentContext.StartActivity(intent);
        }

        void ICalendar.CreateService()
        {
        }

        long GetDateTimeMS(int yr, int month, int day, int hr, int min)
        {
            Calendar c = Calendar.GetInstance(Java.Util.TimeZone.Default);

            c.Set(CalendarField.DayOfMonth, day);
            c.Set(CalendarField.HourOfDay, hr);
            c.Set(CalendarField.Minute, min);
            c.Set(CalendarField.Month, month - 1);
            c.Set(CalendarField.Year, yr);

            return c.TimeInMillis;
        }

        long GetDateTimeMS(DateTime dateTime)
        {
            return GetDateTimeMS(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute);
        }
    }
}