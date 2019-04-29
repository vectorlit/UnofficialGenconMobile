using ConventionMobile.Data;
using ConventionMobile.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConventionMobile.Helpers
{
    public static class GenEventHelpers
    {
        public static string GetFriendlyColumnName(string columnName)
        {
            switch (columnName)
            {
                case "GroupCompany":
                    return "Group/Company";

                case "Title":
                    return "Title";

                case "Description":
                    return "Description";

                case "LongDescription":
                    return "Long Description";

                case "EventType":
                    return "Event Type";

                case "RulesEdition":
                    return "Rules Edition";

                case "GameSystem":
                    return "Game System";

                case "MinimumPlayers":
                    return "Minimum Players";

                case "MaximumPlayers":
                    return "Maximum Players";

                case "MinimumAge":
                    return "Minimum Age";

                case "ExperienceRequired":
                    return "Experience Required";

                case "MaterialsProvided":
                    return "Materials Provided";

                case "StartDateTime":
                    return "Start Time";

                case "Duration":
                    return "Duration";

                case "EndDateTime":
                    return "End Time";

                case "Location":
                    return "Location";

                case "GMs":
                    return "GMs";

                case "WebAddressMoreInfo":
                    return "Web Address/More Info";

                case "EmailAddressMoreInfo":
                    return "Email Address/More Info";

                case "Tournament":
                    return "Tournament";

                case "Prerequisite":
                    return "Prerequisite";

                case "AvailableTickets":
                    return "Available Tickets";

                case "Cost":
                    return "Cost";

                default:
                    return columnName;
            }
        }

        public static List<EventChangeLog> GetChanges(GenEvent oldEvent, GenEvent newEvent, DateTime changeTime)
        {
            var returnMe = new List<EventChangeLog>();

            if (oldEvent.ID != newEvent.ID)
            {
                returnMe.Add(new EventChangeLog
                {
                    ChangeTime = changeTime,
                    Property = "ID",
                    OldValue = oldEvent.ID,
                    NewValue = newEvent.ID,
                    GenEventID = oldEvent.ID
                });
            }

            if (oldEvent.GroupCompany != newEvent.GroupCompany)
            {
                returnMe.Add(new EventChangeLog
                {
                    ChangeTime = changeTime,
                    Property = "GroupCompany",
                    OldValue = oldEvent.GroupCompany,
                    NewValue = newEvent.GroupCompany,
                    GenEventID = oldEvent.ID
                });
            }

            if (oldEvent.Title != newEvent.Title)
            {
                returnMe.Add(new EventChangeLog
                {
                    ChangeTime = changeTime,
                    Property = "Title",
                    OldValue = oldEvent.Title,
                    NewValue = newEvent.Title,
                    GenEventID = oldEvent.ID
                });
            }

            if (oldEvent.Description != newEvent.Description)
            {
                returnMe.Add(new EventChangeLog
                {
                    ChangeTime = changeTime,
                    Property = "Description",
                    OldValue = oldEvent.Description,
                    NewValue = newEvent.Description,
                    GenEventID = oldEvent.ID
                });
            }

            if (oldEvent.LongDescription != newEvent.LongDescription)
            {
                returnMe.Add(new EventChangeLog
                {
                    ChangeTime = changeTime,
                    Property = "LongDescription",
                    OldValue = oldEvent.LongDescription,
                    NewValue = newEvent.LongDescription,
                    GenEventID = oldEvent.ID
                });
            }

            if (oldEvent.EventType != newEvent.EventType)
            {
                returnMe.Add(new EventChangeLog
                {
                    ChangeTime = changeTime,
                    Property = "EventType",
                    OldValue = oldEvent.EventType,
                    NewValue = newEvent.EventType,
                    GenEventID = oldEvent.ID
                });
            }

            if (oldEvent.RulesEdition != newEvent.RulesEdition)
            {
                returnMe.Add(new EventChangeLog
                {
                    ChangeTime = changeTime,
                    Property = "RulesEdition",
                    OldValue = oldEvent.RulesEdition,
                    NewValue = newEvent.RulesEdition,
                    GenEventID = oldEvent.ID
                });
            }

            if (oldEvent.GameSystem != newEvent.GameSystem)
            {
                returnMe.Add(new EventChangeLog
                {
                    ChangeTime = changeTime,
                    Property = "GameSystem",
                    OldValue = oldEvent.GameSystem,
                    NewValue = newEvent.GameSystem,
                    GenEventID = oldEvent.ID
                });
            }

            if (oldEvent.MinimumPlayers != newEvent.MinimumPlayers)
            {
                returnMe.Add(new EventChangeLog
                {
                    ChangeTime = changeTime,
                    Property = "MinimumPlayers",
                    OldValue = oldEvent.MinimumPlayers,
                    NewValue = newEvent.MinimumPlayers,
                    GenEventID = oldEvent.ID
                });
            }

            if (oldEvent.MaximumPlayers != newEvent.MaximumPlayers)
            {
                returnMe.Add(new EventChangeLog
                {
                    ChangeTime = changeTime,
                    Property = "MaximumPlayers",
                    OldValue = oldEvent.MaximumPlayers,
                    NewValue = newEvent.MaximumPlayers,
                    GenEventID = oldEvent.ID
                });
            }

            if (oldEvent.MinimumAge != newEvent.MinimumAge)
            {
                returnMe.Add(new EventChangeLog
                {
                    ChangeTime = changeTime,
                    Property = "MinimumAge",
                    OldValue = oldEvent.MinimumAge,
                    NewValue = newEvent.MinimumAge,
                    GenEventID = oldEvent.ID
                });
            }

            if (oldEvent.ExperienceRequired != newEvent.ExperienceRequired)
            {
                returnMe.Add(new EventChangeLog
                {
                    ChangeTime = changeTime,
                    Property = "ExperienceRequired",
                    OldValue = oldEvent.ExperienceRequired,
                    NewValue = newEvent.ExperienceRequired,
                    GenEventID = oldEvent.ID
                });
            }

            if (oldEvent.MaterialsProvided != newEvent.MaterialsProvided)
            {
                returnMe.Add(new EventChangeLog
                {
                    ChangeTime = changeTime,
                    Property = "MaterialsProvided",
                    OldValue = oldEvent.MaterialsProvided,
                    NewValue = newEvent.MaterialsProvided,
                    GenEventID = oldEvent.ID
                });
            }

            if (oldEvent.StartDateTime != newEvent.StartDateTime)
            {
                returnMe.Add(new EventChangeLog
                {
                    ChangeTime = changeTime,
                    Property = "StartDateTime",
                    OldValue = oldEvent.StartDateTime.ToString(),
                    NewValue = newEvent.StartDateTime.ToString(),
                    GenEventID = oldEvent.ID
                });
            }

            if (oldEvent.Duration != newEvent.Duration)
            {
                returnMe.Add(new EventChangeLog
                {
                    ChangeTime = changeTime,
                    Property = "Duration",
                    OldValue = oldEvent.Duration,
                    NewValue = newEvent.Duration,
                    GenEventID = oldEvent.ID
                });
            }

            if (oldEvent.EndDateTime != newEvent.EndDateTime)
            {
                returnMe.Add(new EventChangeLog
                {
                    ChangeTime = changeTime,
                    Property = "EndDateTime",
                    OldValue = oldEvent.EndDateTime.ToString(),
                    NewValue = newEvent.EndDateTime.ToString(),
                    GenEventID = oldEvent.ID
                });
            }

            if (oldEvent.Location != newEvent.Location)
            {
                returnMe.Add(new EventChangeLog
                {
                    ChangeTime = changeTime,
                    Property = "Location",
                    OldValue = oldEvent.Location,
                    NewValue = newEvent.Location,
                    GenEventID = oldEvent.ID
                });
            }

            if (oldEvent.GMs != newEvent.GMs)
            {
                returnMe.Add(new EventChangeLog
                {
                    ChangeTime = changeTime,
                    Property = "GMs",
                    OldValue = oldEvent.GMs,
                    NewValue = newEvent.GMs,
                    GenEventID = oldEvent.ID
                });
            }

            if (oldEvent.WebAddressMoreInfo != newEvent.WebAddressMoreInfo)
            {
                returnMe.Add(new EventChangeLog
                {
                    ChangeTime = changeTime,
                    Property = "WebAddressMoreInfo",
                    OldValue = oldEvent.WebAddressMoreInfo,
                    NewValue = newEvent.WebAddressMoreInfo,
                    GenEventID = oldEvent.ID
                });
            }

            if (oldEvent.EmailAddressMoreInfo != newEvent.EmailAddressMoreInfo)
            {
                returnMe.Add(new EventChangeLog
                {
                    ChangeTime = changeTime,
                    Property = "EmailAddressMoreInfo",
                    OldValue = oldEvent.EmailAddressMoreInfo,
                    NewValue = newEvent.EmailAddressMoreInfo,
                    GenEventID = oldEvent.ID
                });
            }

            if (oldEvent.Tournament != newEvent.Tournament)
            {
                returnMe.Add(new EventChangeLog
                {
                    ChangeTime = changeTime,
                    Property = "Tournament",
                    OldValue = oldEvent.Tournament,
                    NewValue = newEvent.Tournament,
                    GenEventID = oldEvent.ID
                });
            }
            
            if (oldEvent.Prerequisite != newEvent.Prerequisite)
            {
                returnMe.Add(new EventChangeLog
                {
                    ChangeTime = changeTime,
                    Property = "Prerequisite",
                    OldValue = oldEvent.Prerequisite,
                    NewValue = newEvent.Prerequisite,
                    GenEventID = oldEvent.ID
                });
            }

            if (oldEvent.AvailableTickets != newEvent.AvailableTickets)
            {
                returnMe.Add(new EventChangeLog
                {
                    ChangeTime = changeTime,
                    Property = "AvailableTickets",
                    OldValue = oldEvent.AvailableTickets.ToString(),
                    NewValue = newEvent.AvailableTickets.ToString(),
                    GenEventID = oldEvent.ID
                });
            }

            if (oldEvent.Cost != newEvent.Cost)
            {
                returnMe.Add(new EventChangeLog
                {
                    ChangeTime = changeTime,
                    Property = "Cost",
                    OldValue = oldEvent.Cost,
                    NewValue = newEvent.Cost,
                    GenEventID = oldEvent.ID
                });
            }

            return returnMe;
        }
    }
}
