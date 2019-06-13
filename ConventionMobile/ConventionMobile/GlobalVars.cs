using Acr.UserDialogs;
using ConventionMobile.Data;
using ConventionMobile.Model;
using ConventionMobile.Views;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using ConventionMobile.Business;
using Xamarin.Forms;

namespace ConventionMobile
{
    public class GlobalVars
    {
        public static GenEventsLoadingViewModel GenConBusiness = new GenEventsLoadingViewModel();
        public static GenSearchView View_GenSearchView = null;
        public static GenMapView View_GenMapView = null;
        public static GenUserListView View_GenUserListView = null;
        public static GenEventsLoadingView View_GenEventsLoadingView = null;

        public enum ThemeColors
        {
            Primary = 0,
            Secondary = 1,
            ActionElement = 2
        };

        public readonly static List<Color> ThemeColorsBG = new List<Color>()
        {
            // 2019 Colors: 
            // Primary: Orange
            //new Color(244/255, 178/255, 52/255),
            new Color(0.9568, 0.6980, 0.2039),

            // Secondary: Light Orange
            //new Color(252/255, 205/255, 116/255),
            new Color(0.9882, 0.8039, 0.4549),

            // Action Element: Red
            //new Color(213/255, 36/255, 36/255)
            new Color(0.8352, 0.1412, 0.1412)
        };

        public readonly static List<Color> ThemeColorsText = new List<Color>()
        {
            new Color(0, 0, 0),
            new Color(0, 0, 0),
            new Color(1, 1, 1)
        };

        public static GenconMobileDatabase db
        {
            get
            {
                return _db;
            }
            set
            {
                _db = value;
            }
        }
        private static GenconMobileDatabase _db = null;

        public static List<GlobalOption> allOptions
        {
            get
            {
                return new List<GlobalOption>
                {
                    new GlobalOption("hasSuccessfullyLoadedEvents", hasSuccessfullyLoadedEvents),
                    new GlobalOption("appTitle", appTitle),
                    new GlobalOption("shortTitle", shortTitle),
                    new GlobalOption("navigationTitle", navigationTitle),
                    new GlobalOption("searchTitle", searchTitle),
                    new GlobalOption("userListsTitle", userListsTitle),
                    new GlobalOption("userListsEmptyMessage", userListsEmptyMessage),
                    new GlobalOption("userListSingleEmptyMessage",userListSingleEmptyMessage),
                    new GlobalOption("startingDate", startingDate),
                    new GlobalOption("yearlyStartingDate", yearlyStartingDate),
                    new GlobalOption("GenEventURL", GenEventURL),
                    new GlobalOption("_eventsLastUpdatedPretty", _eventsLastUpdatedPretty),
                    new GlobalOption("_eventsTotalCountPretty", _eventsTotalCountPretty),
                    new GlobalOption("eventsExplanationPretty", eventsExplanationPretty),
                    new GlobalOption("eventsFinalInfoPretty", eventsFinalInfoPretty),
                    new GlobalOption("minSyncTimeSpanMinutes", minSyncTimeSpanMinutes),
                    new GlobalOption("lastSyncTime", lastSyncTime),
                    new GlobalOption("NavigationChoices", NavigationChoices),
                    new GlobalOption("dbVersion", dbVersion),
                    new GlobalOption("lastGlobalVarUpdateTime", lastGlobalVarUpdateTime)
                };
            }
        }

        public enum ToastType
        {
            Green,
            Yellow,
            Red
        }

        public static void DoToast(string toast, ToastType toastType, int duration = 2000)
        {
            var toastConfig = new ToastConfig(toast);
            toastConfig.SetDuration(duration);

            switch (toastType)
            {
                case ToastType.Green:
                    toastConfig.SetBackgroundColor(System.Drawing.Color.FromArgb(12, 220, 25));
                    break;
                case ToastType.Red:
                    toastConfig.SetBackgroundColor(System.Drawing.Color.FromArgb(220, 12, 25));
                    break;
                case ToastType.Yellow:
                    toastConfig.SetBackgroundColor(System.Drawing.Color.FromArgb(193, 193, 25));
                    break;
                default:
                    toastConfig.SetBackgroundColor(System.Drawing.Color.FromArgb(12, 131, 193));
                    break;
            }

            Device.BeginInvokeOnMainThread(() =>
            {
                UserDialogs.Instance.Toast(toastConfig);
            });
        }

        //public static IToastNotificator notifier
        //{
        //    get
        //    {
        //        if (_notifier == null)
        //        {
        //            _notifier = DependencyService.Get<IToastNotificator>();
        //        }
        //        return _notifier;
        //    }
        //}
        //private static IToastNotificator _notifier = null;

        /// <summary>
        /// Returns the last sync time as specified by the server. Since this is UTC, does NOT relate to local last time the user synchronized.
        /// </summary>
        public static async Task<DateTime> serverLastSyncTime()
        {
            return await db.GetLastSyncTime();
        }

        /// <summary>
        /// Returns the last local time the user successfully downloaded events. Does not look at event synctimes, just an internal value.
        /// </summary>
        public static DateTime lastSyncTime
        {
            get
            {
                return getOption<DateTime>("lastSyncTime", yearlyStartingDate);
            }
            set
            {
                setOption("lastSyncTime", value);
            }
        }
        private static DateTime _lastSyncTime = DateTime.MinValue;


        /// <summary>
        /// Returns the last local time the user successfully downloaded global var updates.
        /// </summary>
        public static DateTime lastGlobalVarUpdateTime
        {
            get
            {
                return getOption<DateTime>("lastGlobalVarUpdateTime", yearlyStartingDate);
            }
            set
            {
                setOption("lastGlobalVarUpdateTime", value);
            }
        }
        private static DateTime _lastGlobalVarUpdateTime = DateTime.MinValue;

        /// <summary>
        /// Returns true if a synchronization is necessary.
        /// </summary>
        public static bool isSyncNeeded
        {
            get
            {
                return lastSyncTime.AddMinutes(minSyncTimeSpanMinutes) < DateTime.Now;
            }
        }

        /// <summary>
        /// Returns true if a Global Var synchronization is necessary.
        /// </summary>
        public static bool isGlobalVarSyncNeeded
        {
            get
            {
                return lastGlobalVarUpdateTime.AddMinutes(minSyncTimeSpanMinutes) < DateTime.Now;
            }
        }

        /// <summary>
        /// Used to reset all options to their code-defaults.
        /// </summary>
        public static void resetDefaultOptions()
        {
            GlobalVars.useDefaultOnly = true;
            GlobalVars._navigationChoicesDefault = null;
            GlobalVars.hasSuccessfullyLoadedEvents = GlobalVars.hasSuccessfullyLoadedEvents;
            GlobalVars.appTitle = GlobalVars.appTitle;
            GlobalVars.shortTitle = GlobalVars.shortTitle;
            GlobalVars.navigationTitle = GlobalVars.navigationTitle;
            GlobalVars.userListsTitle = GlobalVars.userListsTitle;
            GlobalVars.userListsEmptyMessage = GlobalVars.userListSingleEmptyMessage;
            GlobalVars.userListSingleEmptyMessage = GlobalVars.userListSingleEmptyMessage;
            GlobalVars.searchTitle = GlobalVars.searchTitle;
            GlobalVars.startingDate = GlobalVars.startingDate;
            GlobalVars.yearlyStartingDate = GlobalVars.yearlyStartingDate;
            GlobalVars.GenEventURL = GlobalVars.GenEventURL;
            GlobalVars._eventsLastUpdatedPretty = GlobalVars._eventsLastUpdatedPretty;
            GlobalVars._eventsTotalCountPretty = GlobalVars._eventsTotalCountPretty;
            GlobalVars.eventsExplanationPretty = GlobalVars.eventsExplanationPretty;
            GlobalVars.eventsFinalInfoPretty = GlobalVars.eventsFinalInfoPretty;
            GlobalVars.minSyncTimeSpanMinutes = GlobalVars.minSyncTimeSpanMinutes;
            GlobalVars.NavigationChoices = GlobalVars.NavigationChoices;
            GlobalVars.lastGlobalVarUpdateTime = GlobalVars.lastGlobalVarUpdateTime;
            GlobalVars.lastSyncTime = GlobalVars.lastSyncTime;
            GlobalVars.GenEventURL = GlobalVars.GenEventURL;
            GlobalVars.useDefaultOnly = false;
        }

        /// <summary>
        /// The minimum time between attempted synchronizations.
        /// </summary>
        public static int minSyncTimeSpanMinutes
        {
            get
            {
                return getOption<int>("minSyncTimeSpanMinutes", 60);
            }
            set
            {
                setOption("minSyncTimeSpanMinutes", value);
            }
        }

        /// <summary>
        /// True when the program has loaded events successfully at least once
        /// </summary>
        public static bool hasSuccessfullyLoadedEvents
        {
            get
            {
                return getOption<bool>("hasSuccessfullyLoadedEvents", false);
            }
            set
            {
                setOption("hasSuccessfullyLoadedEvents", value);
            }
        }

        /// <summary>
        /// True when the activity looking up this variable needs to be reloaded. Will only return "true" once before automatically resetting in order to prevent loops.
        /// This variable is never committed to database.
        /// </summary>
        public static bool isActivityReloadRequested
        {
            get
            {
                bool returnMe = _isActivityReloadRequested;
                _isActivityReloadRequested = false;
                return returnMe;
            }   
            set
            {
                _isActivityReloadRequested = value;
            }
        }
        private static bool _isActivityReloadRequested = false;
        
        /// <summary>
        /// The main application title
        /// </summary>
        public static string appTitle
        {
            get
            {
                return getOption<string>("appTitle", "Unofficial Gen Con 2019");
            }
            set
            {
                setOption("appTitle", value);
            }
        }

        /// <summary>
        /// The main application title, short version
        /// </summary>
        public static string shortTitle
        {
            get
            {
                return getOption<string>("shortTitle", "Gen Con 2019");
            }
            set
            {
                setOption("shortTitle", value);
            }
        }

        private static void setOption(string title, object value, bool commitToDatabase = true)
        {
            var option = cachedObjects.FirstOrDefault(d => d.name == title);
            if (option != null)
            {
                option.value = value;
            }
            else
            {
                NameValueObject newObject = new NameValueObject(title, value);
                cachedObjects.Add(newObject);
                option = newObject;
            }

            if (commitToDatabase)
            {
                //don't care if it blocks this thread cause it's not UI
                Task.Run(async () => 
                {
                    await GlobalVars.db.SetOption(new GlobalOption(title, value));
                });
            }
    
        }

        /// <summary>
        /// This variable can only be manually set. If it is true, then the getOption<T> method will only return DEFAULT values.
        /// USE WITH CAUTION!!
        /// </summary>
        public static bool useDefaultOnly = false;


        /// <summary>
        /// Convenience method for checking cache first then using default value as backup
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="title"></param>
        /// <param name="defaultVal"></param>
        /// <returns></returns>
        private static T getOption<T>(string title, object defaultVal)
        {
            var option = cachedObjects.FirstOrDefault(d => d.name == title);
            if (option != null && !useDefaultOnly)
            {
                return (T)(option.value);
            }
            else
            {
                return (T)defaultVal;
            }
        }

        private static List<NameValueObject> cachedObjects = new List<NameValueObject>();

        /// <summary>
        /// The navigation title
        /// </summary>
        public static string navigationTitle
        {
            get
            {
                return getOption<string>("navigationTitle", "Maps/Info");
            }
            set
            {
                setOption("navigationTitle", value);
            }
        }

        /// <summary>
        /// The title for the user specific lists tab
        /// </summary>
        public static string userListsTitle
        {
            get
            {
                return getOption<string>("userListsTitle", "My Lists");
            }
            set
            {
                setOption("userListsTitle", value);
            }
        }

        /// <summary>
        /// The navigation title
        /// </summary>
        public static string searchTitle
        {
            get
            {
                return getOption<string>("searchTitle", "Search Events");
            }
            set
            {
                setOption("searchTitle", value);
            }
        }

        /// <summary>
        /// The current starting date for this year.
        /// </summary>
        public static DateTime startingDate
        {
            get
            {
                return getOption<DateTime>("startingDate", new DateTime(2019, 7, 31, 0, 0, 0));
            }
            set
            {
                setOption("startingDate", value);
            }
        }

        /// <summary>
        /// Yearly starting date, only to be used for event delineating between years.
        /// </summary>
        public static DateTime yearlyStartingDate
        {
            get
            {
                return getOption<DateTime>("yearlyStartingDate", new DateTime(2019, 4, 30));
            }
            set
            {
                setOption("yearlyStartingDate", value);
            }
        }

        /// <summary>
        /// The base URL for the web api requests
        /// </summary>
        public static string GenEventURL
        {
            get
            {
                return getOption<string>("GenEventURL", "https://unofficialgen.co/GenConEvents");
            }
            set
            {
                setOption("GenEventURL", value);
            }
        }


        /// <summary>
        /// The base URL for UserEventList-based web api requests (POST)
        /// </summary>
        public static string UserEventListURL
        {
            get
            {
                return getOption<string>("UserEventListURL", "https://unofficialgen.co");
            }
            set
            {
                setOption("UserEventListURL", value);
            }
        }
        

        /// <summary>
        /// The base URL for the web api requests for GLOBAL OPTIONS
        /// </summary>
        public static string GlobalOptionsURL
        {
            get
            {
                return getOption<string>("GlobalOptionsURL", "https://unofficialgen.co/GenConMobile");
            }
            set
            {
                setOption("GlobalOptionsURL", value);
            }
        }

        /// <summary>
        /// Convenience URL to retrieve global options and maps after a certain sync_time (use String_Format and replace {0} with date)
        /// If you want to only return a specific # of results, add "/{1}" after this string, where {1} is the integer amount of desired results
        /// </summary>
        public static string GlobalOptionsURLCustomizableURL
        {
            get
            {
                return GlobalOptionsURL + getOption<string>("GlobalOptionsURLCustomizableURL", "/timedelay/{0}");
            }
            set
            {
                setOption("GlobalOptionsURLCustomizableURL", value);
            }
        }

        /// <summary>
        /// Convenience URL to retrieve events after a certain sync_time (use String_Format and replace {0} with date)
        /// If you want to only return a specific # of results, add "/{1}" after this string, where {1} is the integer amount of desired results
        /// </summary>
        public static string GenEventAllEventsCustomizableURL
        {
            get
            {
                return GenEventURL + getOption<string>("GenEventAllEventsCustomizableURL", "/timedelay/{0}");
            }
            set
            {
                setOption("GenEventAllEventsCustomizableURL", value);
            }
        }

        /// <summary>
        /// Convenience URL to retrieve count of all events
        /// </summary>
        public static string GenEventAfterDateEventsCountURL(DateTime? lastSyncTime = null)
        {
            lastSyncTime = lastSyncTime == null ? yearlyStartingDate : lastSyncTime;
            return String.Format(GenEventAllEventsCustomizableURL, ((DateTime)lastSyncTime).ToString("yyyy-MM-dd't'HH:mm:ss")) + getOption<string>("GenEventAllEventsCountURL", "/numResults");
        }

        /// <summary>
        /// Color of text regarding money
        /// </summary>
        public static Color colorMoney = Color.FromRgb(0, 0.8, 0);

        /// <summary>
        /// Color of text links
        /// </summary>
        public static Color colorLink = Color.FromHex("5EA1FF");

        /// <summary>
        /// Color of text for available tickets
        /// </summary>
        public static Color colorTicketsAvailable = Color.Default;

        /// <summary>
        /// Color of text for unavailable tickets
        /// </summary>
        public static Color colorTicketsUnavailable = Color.FromRgb(0.7, 0, 0);

        /// <summary>
        /// Color of background for available tickets
        /// </summary>
        public static Color bgColorTicketsAvailable = Color.FromRgb(0.95, 0.95, 0.95);

        /// <summary>
        /// Color of background for unavailable tickets
        /// </summary>
        public static Color bgColorTicketsUnavailable = Color.FromRgb(0.5, 0.5, 0.5);


        /// <summary>
        /// Color of background for user-list-items that have changed in recent update
        /// </summary>
        public static Color bgColorChangesDetected = Color.FromRgb(0.88, 0.87, 0.01);
        

        public static double sizeLarge
        {
            get
            {
                if (_sizeLarge == 0)
                {
                    _sizeLarge = Device.GetNamedSize(NamedSize.Large, typeof(Label));
                }
                return _sizeLarge;
            }
            set
            {
                _sizeLarge = value;
            }
        }
        private static double _sizeLarge = 0;

        public static double sizeMedium
        {
            get
            {
                if (_sizeMedium == 0)
                {
                    _sizeMedium = Device.GetNamedSize(NamedSize.Medium, typeof(Label));
                }
                return _sizeMedium;
            }
            set
            {
                _sizeMedium = value;
            }
        }
        private static double _sizeMedium = 0;

        public static double sizeSmall
        {
            get
            {
                if (_sizeSmall == 0)
                {
                    _sizeSmall = Device.GetNamedSize(NamedSize.Small, typeof(Label));
                }
                return _sizeSmall;
            }
            set
            {
                _sizeSmall = value;
            }
        }
        private static double _sizeSmall = 0;

        public static double sizeListCellHeight
        {
            get
            {
                if (_sizeListCellHeight == 0)
                {
                    _sizeListCellHeight = Device.GetNamedSize(NamedSize.Large, typeof(Label)) * 3;
                }
                return _sizeListCellHeight;
            }
            set
            {
                _sizeListCellHeight = value;
            }
        }
        private static double _sizeListCellHeight = 0;


        /// <summary>
        /// The message to show in the empty user lists box
        /// </summary>
        public static string userListsEmptyMessage
        {
            get
            {
                return getOption<string>("userListsEmptyMessage", "This is where your custom user lists are located. Add a new list above and begin adding events to get started!");
            }
            set
            {
                setOption("userListsEmptyMessage", value);
            }
        }

        /// <summary>
        /// The message to show in the empty user lists box
        /// </summary>
        public static string userListSingleEmptyMessage
        {
            get
            {
                return getOption<string>("userListSingleEmptyMessage", $"This list is currently empty. Search for events from the \"{searchTitle}\" tab, and begin adding them to this list.");
            }
            set
            {
                setOption("userListSingleEmptyMessage", value);
            }
        }

        /// <summary>
        /// The topmost string to display for events loading
        /// </summary>
        public static string eventsLastUpdatedPretty
        {
            get
            {
                return String.Format(_eventsLastUpdatedPretty, lastSyncTime);
            }
        }
        public static string _eventsLastUpdatedPretty
        {
            get
            {
                return getOption<string>("_eventsLastUpdatedPretty", "Up-to-date as of: {0}");
            }
            set
            {
                setOption("_eventsLastUpdatedPretty", value);
            }
        }

        /// <summary>
        /// Returns count of events. Cached only starting after database initialization.
        /// </summary>
        public static int eventCount
        {
            get
            {
                return getOption<int>("eventCount", 0);
            }
            set
            {
                setOption("eventCount", value, false);
            }
        }

        /// <summary>
        /// Returns the current database version.
        /// </summary>
        public static int dbVersion
        {
            get
            {
                return getOption<int>("dbVersion", 0);
            }
            set
            {
                setOption("dbVersion", value);
            }
        }

        /// <summary>
        /// The second string to display for events loading.
        /// </summary>
        public static string eventsTotalCountPretty
        {
            get
            {
                return String.Format(_eventsTotalCountPretty, GlobalVars.eventCount);
            }
        }
        public static string _eventsTotalCountPretty
        {
            get
            {
                return getOption<string>("_eventsTotalCountPretty", "There are currently {0} events");
            }
            set
            {
                setOption("_eventsTotalCountPretty", value);
            }
        }

        /// <summary>
        /// The third string to display for events loading (explanation)
        /// </summary>
        public static string eventsExplanationPretty
        {
            get
            {
                return getOption<string>("eventsExplanationPretty", "Search for events above, or click the button below to show all events for the selected day. (There are tons of events, you've been warned!)");
            }
            set
            {
                setOption("eventsExplanationPretty", value);
            }
        }

        /// <summary>
        /// The final info string for events loading (probably promotional)
        /// </summary>
        public static string eventsFinalInfoPretty
        {
            get
            {
                return getOption<string>("eventsFinalInfoPretty", "** Food trucks and other outstanding information will be added/updated to 2019 as soon as Gen Con releases them!\r\n\r\nThis app is NOT officially endorsed by Gen Con. It is a fan-created app service, provided for free.");
            }
            set
            {
                setOption("eventsFinalInfoPretty", value);
            }
        }

        /// <summary>
        /// List of all 3-letter prefixes used in event IDs - to be used in regex
        /// </summary>
        public static string EventIDPrefixesRegexPattern
        {
            get
            {
                return getOption<string>("EventIDPrefixesRegexPattern", "^((ani|bgm|cgm|egm|ent|flm|hmn|kid|lrp|mhe|nmn|rpg|sem|spa|tcg|tda|trd|wks|zed)\\d+)$");
            }
            set
            {
                setOption("EventIDPrefixesRegexPattern", value);
            }
        }

        /// <summary>
        /// Returns true if the input string appears to be an attempt at typing an event ID
        /// </summary>
        /// <param name="input">The typed string</param>
        /// <returns></returns>
        public static bool isTypedEventID(string input)
        {
            return Regex.IsMatch(input, EventIDPrefixesRegexPattern, RegexOptions.IgnoreCase);
        }
        
        /// <summary>
        /// Items to be used on the main navigation page
        /// </summary>
        public static List<DetailChoice> NavigationChoices
        {
            get
            {
                if (_navigationChoicesDefault == null)
                {
                    _navigationChoicesDefault = JsonConvert.SerializeObject(new List<DetailChoice>
                        {
                            new DetailChoice ("Convention Floor 1", "convention-1.jpg", typeof(MapViewPage), true, "ic_map_black_24dp.png"),
                            new DetailChoice ("Convention Floor 2", "convention-2.jpg", typeof(MapViewPage), true, "ic_map_black_24dp.png"),
                            new DetailChoice ("Exhibit Hall", "exhibithallmap2019.jpg", typeof(MapViewPage), true, "ic_map_black_24dp.png"),
                            new DetailChoice ("Parking", "Parking.jpg", typeof(MapViewPage), true, "ic_map_black_24dp.png"),
                            new DetailChoice ("Skywalk/Downtown", "Skywalk-DTHotel.jpg", typeof(MapViewPage), true, "ic_map_black_24dp.png"),
                            new DetailChoice ("Crowne Plaza", "crowneplaza.jpg", typeof(MapViewPage), true, "ic_map_black_24dp.png"),
                            new DetailChoice ("Embassy Suites", "embassysuites.jpg", typeof(MapViewPage), true, "ic_map_black_24dp.png"),
                            new DetailChoice ("Hyatt Regency", "hyattregency.jpg", typeof(MapViewPage), true, "ic_map_black_24dp.png"),
                            new DetailChoice ("Lucas Oil Stadium", "lucasoilstadium.jpg", typeof(MapViewPage), true, "ic_map_black_24dp.png"),
                            new DetailChoice ("Marriott", "marriott.svg", typeof(MapViewPage), true, "ic_map_black_24dp.png"),
                            new DetailChoice ("JW Marriott", "jwmarriott.svg", typeof(MapViewPage), true, "ic_map_black_24dp.png"),
                            new DetailChoice ("Omni", "omni.jpg", typeof(MapViewPage), true, "ic_map_black_24dp.png"),
                            new DetailChoice ("Westin", "westin.svg", typeof(MapViewPage), true, "ic_map_black_24dp.png"),
                            new DetailChoice ("Union Station", "unionstation.jpg", typeof(MapViewPage), true, "ic_map_black_24dp.png"),
                            new DetailChoice ("Vegan/Vegetarian Guide", "vegan.html", typeof(MapViewPage), true, "ic_directions_black_24dp.png"),
                            new DetailChoice ("Food Trucks", "foodtrucks.html", typeof(MapViewPage), true, "ic_directions_black_24dp.png"),
                            new DetailChoice ("Driving Directions", "DrivingDirections.html", typeof(MapViewPage), true, "ic_directions_black_24dp.png"),
                            new DetailChoice ("Interactive Online Map", "https://www.gencon.com/map?lt=13.81674404684894&lg=37.705078125&f=1&z=5", typeof(MapViewPage), true, "ic_public_black_24dp.png")
                        });
                }

                string dbItem = getOption<string>("NavigationChoices", _navigationChoicesDefault);

                return JsonConvert.DeserializeObject<List<DetailChoice>>(dbItem);
            }
            set
            {
                setOption("NavigationChoices", JsonConvert.SerializeObject(value));
            }
        }

        private static string _navigationChoicesDefault = null;

        public static async Task ImportUserEventList(string data, string name)
        {
            if (hasSuccessfullyLoadedEvents && eventCount > 0)
            {
                var newList = new UserEventList
                {
                    Title = name
                };
                var splitData = data.Split(';').ToList();
                foreach (var splitItem in splitData)
                {
                    var addEvent = await db.GetGenEventAsync(splitItem);
                    if (addEvent != null)
                    {
                        newList.Events.Add(addEvent);
                    }
                }

                // If we have some events we should now be successful
                if (newList.Events.Count > 0 && name.Length > 0)
                {
                    await db.UpdateUserEventListWithChildrenAsync(newList);
                    DoToast("Successfully imported list " + name.ToString() + "!", ToastType.Green, 5000);
                    try
                    {
                        View_GenUserListView.IsUpdateRequested = true;
                    }
                    catch (Exception) { }
                }
            }
        }

        /// <summary>
        /// Returns the a formatted HTML App Deep Link (genconevents://) for the given list.
        /// Returns the link itself with some "click here" text, surrounded by a "a" tag.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string GenerateUserEventListLinkHTML(UserEventList list)
        {
            StringBuilder returnMe = new StringBuilder();
            returnMe.Append("<a href=\"genconevents://list?name=");

            var newName = list.Title;

            Regex rgx = new Regex("[^a-zA-Z0-9 -_]");
            newName = rgx.Replace(newName, "");

            returnMe.Append(HttpUtility.UrlEncode(newName));

            returnMe.Append("&data=");

            for (int i = 0; i < list.Events.Count; i++)
            {
                if (i == list.Events.Count - 1)
                {
                    returnMe.Append(list.Events[i].ID);
                }
                else
                {
                    returnMe.Append(list.Events[i].ID + "%3B");
                }
            }

            returnMe.Append($"\" target=\"_blank\">Tap here on a device with \"{appTitle}\" installed to import the event list.</a>");

            return returnMe.ToString();
        }

        /// <summary>
        /// Returns the a formatted App Deep Link (genconevents://) for the given list.
        /// Returns ONLY the link itself, no HTML or text
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string GenerateUserEventListLink(UserEventList list)
        {
            StringBuilder returnMe = new StringBuilder();
            returnMe.Append("genconevents://list?name=");

            var newName = list.Title;

            Regex rgx = new Regex("[^a-zA-Z0-9 -_]");
            newName = rgx.Replace(newName, "");

            returnMe.Append(HttpUtility.UrlEncode(newName));

            returnMe.Append("&data=");

            for (int i = 0; i < list.Events.Count; i++)
            {
                if (i == list.Events.Count - 1)
                {
                    returnMe.Append(list.Events[i].ID);
                }
                else
                {
                    returnMe.Append(list.Events[i].ID + "%3B");
                }
            }

            return returnMe.ToString();
        }

        public static DetailChoice GetMapName(string location)
        {
            if (string.IsNullOrEmpty(location))
            {
                location = "";
            }

            var splitLoc = location.Split(':');

            string loc1 = "";

            if (splitLoc.Length >= 1)
            {
                loc1 = splitLoc[0].Trim().ToLower();
            }

            if (loc1.Contains("crowne"))
            {
                return (NavigationChoices.FirstOrDefault(d => d.data.Contains("crowne")));
            }
            else if (loc1.Contains("embassy"))
            {
                return (NavigationChoices.FirstOrDefault(d => d.data.Contains("embassy")));
            }
            else if (loc1.Contains("hyatt"))
            {
                return (NavigationChoices.FirstOrDefault(d => d.data.Contains("hyatt")));
            }
            else if (loc1.StartsWith("icc"))
            {
                for (var i = 1; i < splitLoc.Length; i++)
                {
                    var finalSplit = splitLoc[i].Split(' ');
                    for (var j = 0; j < finalSplit.Length; j++)
                    {
                        if (Regex.IsMatch(finalSplit[j].Trim(),"^([2]\\d{2,})") || finalSplit[j].Contains("2nd"))
                        {
                            return (NavigationChoices.FirstOrDefault(d => d.data.Contains("convention-2")));
                        }
                    }
                }

                return (NavigationChoices.FirstOrDefault(d => d.data.Contains("convention-1")));
            }
            else if (loc1.Contains("jw"))
            {
                return (NavigationChoices.FirstOrDefault(d => d.data.Contains("jw")));
            }
            else if (loc1.StartsWith("los") || loc1.Contains("lucas"))
            {
                return (NavigationChoices.FirstOrDefault(d => d.data.Contains("lucas")));
            }
            else if (loc1.Contains("omni"))
            {
                return (NavigationChoices.FirstOrDefault(d => d.data.Contains("omni")));
            }
            else if (loc1.Contains("union"))
            {
                return (NavigationChoices.FirstOrDefault(d => d.data.Contains("union")));
            }
            else if (loc1.Contains("westin"))
            {
                return (NavigationChoices.FirstOrDefault(d => d.data.Contains("westin")));
            }

            return null;
        }

        public static void AddToCalendar(GenEvent genEvent)
        {
            DependencyService.Get<ICalendar>().AddToCalendar(genEvent);
        }

        public static event EventHandler FileDownloadProgressUpdated = delegate { };

        private static void FileService_FileDownloadProgressUpdated(object sender, EventArgs e)
        {
            OnFileDownloadProgressUpdated((FileDownloadUpdateEventArgs)e);
        }

        private static void OnFileDownloadProgressUpdated(FileDownloadUpdateEventArgs e)
        {
            FileDownloadProgressUpdated?.Invoke(null, e);
        }

        public static async Task<bool> OverwriteOptions(string downloadedData)
        {
            try
            {
                var data = JArray.Parse(downloadedData);

                string type = null;
                string val = null;
                string title = null;

                bool committedStuff = false;

                if (data != null)
                {
                    var errorOccurred = false;

                    foreach (JObject d in data.Children<JObject>())
                    {
                        if (d.Property("files") != null)
                        {
                            List<string> fileURLs = new List<string>();
                            List<string> fileNames = new List<string>();

                            var files = (JArray)d["files"];

                            foreach (JObject file in files.Children<JObject>())
                            {
                                try
                                {
                                    fileURLs.Add((string)file["val"]);
                                    fileNames.Add((string)file["title"]);
                                    committedStuff = true;
                                }
                                catch (Exception e)
                                {
                                    string message = e.Message;
                                    errorOccurred = true;
                                }
                            }

                            try
                            {
                                FileManager fileManager = new FileManager(new FileService());

                                fileManager.FileDownloadProgressUpdated += FileService_FileDownloadProgressUpdated;

                                List<bool> results = await fileManager.DownloadFiles(fileURLs, fileNames);

                                foreach (var fileResult in results)
                                {
                                    errorOccurred &= !fileResult;
                                    committedStuff = true;
                                }

                                fileManager.FileDownloadProgressUpdated -= FileService_FileDownloadProgressUpdated;
                            }
                            catch (Exception ex)
                            {
                                string message = ex.Message;
                                errorOccurred = true;

                            }
                        }

                        if (!errorOccurred && d.Property("items") != null)
                        {
                            var items = (JArray)d["items"];
                            foreach (JObject item in items.Children<JObject>())
                            {
                                try
                                {
                                    type = (string)item["type"];
                                    val = (string)item["val"];
                                    title = (string)item["title"];
                                    

                                    if (type != null && val != null && title != null)
                                    {
                                        if (title == "NavigationChoices")
                                        {
                                            val = val.Replace("GenconMobile.Resources.", "");
                                            GlobalVars.NavigationChoices = JsonConvert.DeserializeObject<List<DetailChoice>>(val); 
                                            committedStuff = true;
                                        }
                                        else
                                        {
                                            if (type == "System.String")
                                            {
                                                setOption(title, ((string)val).Replace("\\r","\r").Replace("\\n","\n"));
                                                committedStuff = true;
                                            }
                                            else if (type == "System.Boolean")
                                            {
                                                setOption(title, bool.Parse(val));
                                                committedStuff = true;
                                            }
                                            else if (type == "System.Int32")
                                            {
                                                setOption(title, int.Parse(val));
                                                committedStuff = true;
                                            }
                                            else if (type == "System.DateTime")
                                            {
                                                setOption(title, new DateTime(long.Parse(val)));
                                                committedStuff = true;
                                            }

                                        }
                                    }


                                    type = null;
                                    val = null;
                                    title = null;

                                }
                                catch (Exception)
                                {
                                    errorOccurred = true;
                                    string error = "Type: " + (type == null ? "" : type)
                                        + "Value: " + (val == null ? "" : val.ToString())
                                        + "Title: " + (title == null ? "" : title.ToString());
                                }

                            }
                        }

                        if (!errorOccurred && committedStuff)
                        {
                            //For some reason this doesn't commit to DB. FIXME
                            lastGlobalVarUpdateTime = TimeZoneInfo.ConvertTime(new DateTime(long.Parse((string)d["updateDate"])).AddMinutes(2), TimeZoneInfo.Local);
                        }
                        else
                        {
                            throw new Exception("whoops");
                        }

                    }

                    if (!errorOccurred && committedStuff)
                    {
                        lastGlobalVarUpdateTime = DateTime.Now;
                        return true;
                    }
                    else
                    {
                        throw new Exception("whoops");
                    }
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                message += "";
                
                //problem downloading or deserializing data
            }

            return false;
        }

        public static T TryCast<T>(object input)
        {
            return (T)input;
        }

    }

    /// <summary>
    /// Class which holds an object and associated name.
    /// </summary>
    public class NameValueObject
    {
        public string name { get; set; }
        public object value { get; set; }

        public NameValueObject()
        {

        }

        public NameValueObject(string name, object value)
        {
            this.name = name;
            this.value = value;
        }
    }

    /// <summary>
    /// A line item to be displayed on the navigation detail page
    /// </summary>
    public class DetailChoice
    {
        /// <summary>
        /// The "Title" of the choice (displayed on screen)
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// The actual "data" of the choice (not displayed, used for data binding)
        /// </summary>
        public string data { get; set; }

        [JsonIgnore]
        /// <summary>
        /// The type of page we want to pass the data choice to
        /// </summary>
        public Type pageType
        {
            get
            {
                return Type.GetType(_type);
            }
            set
            {
                _type = value.FullName;
            }
        }
        
        public string _type { get; set; }

        /// <summary>
        /// The image to display alongside the title (optional)
        /// </summary>
        public string image { get; set; }

        public bool isPage { get; set; }

        public DetailChoice()
        {
            this.isPage = false;
        }

        public DetailChoice(string title, string data, Type pageType, bool isPage, string image = "")
        {
            this.title = title;
            this.data = data;
            this.pageType = pageType;
            this.image = image;
            this.isPage = isPage;
        }
    }
}

