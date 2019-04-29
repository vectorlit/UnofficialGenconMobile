using ConventionMobile.Model;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace ConventionMobile.Data
{
    public class GenEventUserEventList
    {
        [ForeignKey(typeof(UserEventList))]
        public int UserEventListID { get; set;}

        [ForeignKey(typeof(GenEvent))]
        public string GenEventID { get; set; }
    }

    public class UserEventList : INotifyPropertyChanged
    {
        [PrimaryKey]
        [AutoIncrement]
        [NotNull]
        public int ID
        {
            get
            {
                return _ID;
            }
            set
            {
                SetField(ref _ID, value, "ID");
            }
        }
        private int _ID;

        public string ExternalAddress { get; set; } = null;
        public string InternalSecret { get; set; } = null;

        [Collation("NOCASE")]
        public string Title
        {
            get
            {
                return _Title;
            }
            set
            {
                SetField(ref _Title, value, "Title");
            }
        }
        private string _Title;

        [ManyToMany(typeof(GenEventUserEventList))]
        public List<GenEvent> Events { get; set; } = new List<GenEvent>();

        public string eventsBlobbed { get; set; }

        public bool HasEventListChangedSinceSync { get; set; } = true;

        public UserEventList()
        {
            
        }

        [Ignore]
        public string PublicURL
        {
            get
            {
                return GlobalVars.UserEventListURL + "/n/" + ExternalAddress;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
