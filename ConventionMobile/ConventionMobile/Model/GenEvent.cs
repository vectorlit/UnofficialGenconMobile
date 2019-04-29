using System;
using SQLite;
using System.ComponentModel;
using Xamarin.Forms;
using System.Linq;
using SQLiteNetExtensions.Attributes;
using ConventionMobile.Data;
using System.Collections.Generic;

namespace ConventionMobile.Model
{
    /// <summary>
    /// Default model for Gencon Events. Update from genCatalog when necessary (set up link later)
    /// </summary>
    public partial class GenEvent : INotifyPropertyChanged
    {
        [Ignore]
        public int docid
        {
            get
            {
                return int.Parse(ID.Substring(5, ID.Length - 5));
            }
        }

        [PrimaryKey]
        [Collation("NOCASE")]
        public string ID { get; set; }

        public string GroupCompany { get; set; }

        [Collation("NOCASE")]
        public string Title { get; set; }

        [Collation("NOCASE")]
        public string Description { get; set; }

        [Collation("NOCASE")]
        public string LongDescription { get; set; }

        public string EventType { get; set; }

        public string RulesEdition { get; set; }

        public string GameSystem { get; set; }

        public string MinimumPlayers { get; set; }

        public string MaximumPlayers { get; set; }

        public string MinimumAge { get; set; }

        public string ExperienceRequired { get; set; }

        public string MaterialsProvided { get; set; }

        public bool isMaterialsProvided { get; set; }

        public DateTime StartDateTime { get; set; }

        public string Duration { get; set; }

        public DateTime EndDateTime { get; set; }

        public string Location { get; set; }

        public string GMs { get; set; }

        public string WebAddressMoreInfo { get; set; }

        public string EmailAddressMoreInfo { get; set; }

        public string Tournament { get; set; }

        public bool isTournament { get; set; }

        public string Prerequisite { get; set; }

        public int AvailableTickets { get; set; }

        public DateTime TicketsSyncTime { get; set; }


        public bool HasUpdateNotifications { get; set; } = false;


        [ManyToMany(typeof(GenEventUserEventList))]
        public List<UserEventList> UserEventLists { get; set; } = new List<UserEventList>();


        [Ignore]
        public string FormattedUpdateTime
        {
            get
            {
                return String.Format("Last Updated: {0}\r\nIf this date is old, it could mean the event has simply not changed since then (no one has purchased tickets, etc)",
                    TicketsSyncTime.ToString());
            }
        }

        [Ignore]
        public string FormattedPlayers
        {
            get
            {
                return String.Format("{0}-{1} Players", MinimumPlayers, MaximumPlayers);
            }
        }

        [Ignore]
        public string FormattedDate
        {
            get
            {
                double hrs = Math.Round((this.EndDateTime - this.StartDateTime).TotalHours, 2);

                return String.Format("{0}@{1}({2}hr{3})",
                    this.StartDateTime.DayOfWeek.ToString().Substring(0, 3),
                    this.StartDateTime.ToString("HH:mm"),
                    hrs.ToString(),
                    hrs == 1 ? "" : "s"
                    );
            }
        }

        public string Cost { get; set; }

        public string FormattedCost
        {
            get
            {
                return String.Format("${0}", Cost);
            }
        }

        public double parsedCost { get; set; }

        public string LiveURL { get; set; }

        public DateTime SyncTime { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        
        private Color _backgroundColor;

        [Ignore]
        public Color BackgroundColor
        {
            get
            {
                if (IsSelected) //we're on a selectable list, and it should show the "selected" background
                {
                    return GlobalVars.bgColorTicketsUnavailable;
                }
                else if (HasUpdateNotifications)
                {
                    return GlobalVars.bgColorChangesDetected;
                }
                else if (!hasColorBeenSet)
                {
                    hasColorBeenSet = true;
                    if (AvailableTickets > 0)
                    {
                        BackgroundColor = GlobalVars.bgColorTicketsAvailable;
                    }
                    else
                    {
                        BackgroundColor = GlobalVars.bgColorTicketsUnavailable;
                    }
                }
                return _backgroundColor;
            }
            set
            {
                hasColorBeenSet = true;
                _backgroundColor = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BackgroundColor"));
            }
        }
        
        private bool hasColorBeenSet = false;
        
        [Ignore]
        public bool IsSelected
        {
            get
            {
                return _IsSelected;
            }
            set
            {
                _IsSelected = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsSelected"));
            }
        }
        private bool _IsSelected = false;

        [Ignore]
        public bool IsLongPressSelected
        {
            get
            {
                return _IsLongPressSelected;
            }
            set
            {
                _IsLongPressSelected = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsLongPressSelected"));
                OnLongPressChanged?.Invoke(this, new LongPressChangedEventArgs(this));
            }
        }
        public bool _IsLongPressSelected;

        public event LongPressChangedEventHandler OnLongPressChanged;

        public delegate void LongPressChangedEventHandler(object sender, LongPressChangedEventArgs e);

        [Ignore]
        public bool IsTapped
        {
            get
            {
                return _IsTapped;
            }
            set
            {
                _IsTapped = value;
                OnIsTapped?.Invoke(this, new TappedEventArgs(null));
            }
        }
        public bool _IsTapped;

        public event IsTappedChangedEventHandler OnIsTapped;

        public delegate void IsTappedChangedEventHandler(object sender, TappedEventArgs e);

        public class LongPressChangedEventArgs : EventArgs
        {
            public GenEvent genEvent { get; set; }
            public LongPressChangedEventArgs(GenEvent genEvent)
            {
                this.genEvent = genEvent;
            }
        }

        //public void SetColors(bool isSelected)
        //{
        //    //right now these two are identical, but we can actually choose to change this later if we want.
        //    if (isSelected)
        //    {
        //        if (AvailableTickets > 0)
        //        {
        //            BackgroundColor = GlobalVars.bgColorTicketsAvailable;
        //        }
        //        else
        //        {
        //            BackgroundColor = GlobalVars.bgColorTicketsUnavailable;
        //        }
        //    }
        //    else
        //    {
        //        if (AvailableTickets > 0)
        //        {
        //            BackgroundColor = GlobalVars.bgColorTicketsAvailable;
        //        }
        //        else
        //        {
        //            BackgroundColor = GlobalVars.bgColorTicketsUnavailable;
        //        }
        //    }
        //}
    }
}
