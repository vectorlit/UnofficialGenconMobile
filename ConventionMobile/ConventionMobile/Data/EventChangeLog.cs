using ConventionMobile.Model;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConventionMobile.Data
{
    public class EventChangeLog
    {
        [PrimaryKey]
        [AutoIncrement]
        public int ID { get; set; }
        public string GenEventID { get; set; }

        public string Property { get; set; } = null;

        public string NewValue { get; set; } = null;
        public string OldValue { get; set; } = null;

        public DateTime ChangeTime { get; set; } = DateTime.MinValue;
    }
}
