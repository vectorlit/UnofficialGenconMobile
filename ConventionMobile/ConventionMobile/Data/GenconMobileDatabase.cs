using ConventionMobile.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Threading;
using System.Reflection;
using Newtonsoft.Json;
using MoreLinq;
using SQLite;
using System.Collections.ObjectModel;
using SQLiteNetExtensionsAsync.Extensions;
using ConventionMobile.Helpers;

namespace ConventionMobile.Data
{
    public class GenconMobileDatabase
    {
        // private static readonly AsyncLock _lock = new AsyncLock();

        SQLiteAsyncConnection database;

        public enum DBOptions
        {
            SortTime,
            SortTitle,
            SortTickets,
            SortPrice,

            FilterWednesday,
            FilterThursday,
            FilterFriday,
            FilterSaturday,
            FilterSunday,
            FilterAllDays,

            SortAscending,
            SortDescending,

            After00,
            After01,
            After02,
            After03,
            After04,
            After05,
            After06,
            After07,
            After08,
            After09,
            After10,
            After11,
            After12,
            After13,
            After14,
            After15,
            After16,
            After17,
            After18,
            After19,
            After20,
            After21,
            After22,
            After23,
            Before01,
            Before02,
            Before03,
            Before04,
            Before05,
            Before06,
            Before07,
            Before08,
            Before09,
            Before10,
            Before11,
            Before12,
            Before13,
            Before14,
            Before15,
            Before16,
            Before17,
            Before18,
            Before19,
            Before20,
            Before21,
            Before22,
            Before23,
            Before24
        }
        
        private GenconMobileDatabase()
        {
            database = DependencyService.Get<ISQLite>().GetConnection();
        }

        public static GenconMobileDatabase Create()
        {
            var genconMobileDatabase = new GenconMobileDatabase();

            genconMobileDatabase.Initialize();
            return genconMobileDatabase;

        }

        private void Initialize()
        {
            database.CreateTableAsync<GlobalOption>();
            database.CreateTableAsync<GenEvent>(CreateFlags.FullTextSearch4);
            database.CreateTableAsync<UserEventList>();
            database.CreateTableAsync<GenEventUserEventList>();
            database.CreateTableAsync<EventChangeLog>();

            GlobalVars.db = this;

            InitDatabase();
            CheckDBVersion();
        }

        private void CheckDBVersion()
        {
            if (GlobalVars.dbVersion <= 6)
            {
                //delete tables
                try
                {
                    database.DropTableAsync<EventChangeLog>().Wait();
                }
                catch (Exception) { }
                database.DropTableAsync<GenEvent>().Wait();
                database.DropTableAsync<GlobalOption>().Wait();
                try
                {
                    database.DropTableAsync<UserEventList>().Wait();
                }
                catch (Exception) { }
                try
                {
                    database.DropTableAsync<GenEventUserEventList>().Wait();
                }
                catch (Exception) { }


                database.CreateTableAsync<GenEvent>(CreateFlags.FullTextSearch4).Wait();
                database.CreateTableAsync<GlobalOption>().Wait();
                database.CreateTableAsync<UserEventList>().Wait();
                database.CreateTableAsync<GenEventUserEventList>().Wait();
                database.CreateTableAsync<EventChangeLog>().Wait();

                GlobalVars.db = this;
                InitDatabase();


                GlobalVars.resetDefaultOptions();
                GlobalVars.dbVersion = 7;

                GlobalVars.isActivityReloadRequested = true;
            }
            //else if (GlobalVars.dbVersion == 4)
            //{
            //    //reset only options for this version
            //    await database.DropTableAsync<GlobalOption>();
            //    await database.CreateTableAsync<GlobalOption>();
            //    GlobalVars.db = this;
            //    await initDatabase();
            //    GlobalVars

            //    GlobalVars.resetDefaultOptions();

            //    GlobalVars.lastSyncTime = await GlobalVars.serverLastSyncTime();

            //    GlobalVars.dbVersion = 5;

            //    GlobalVars.isActivityReloadRequested = true;
            //}
        }

        private void InitDatabase()
        {
            var allNewOptions = GlobalVars.allOptions;
            Type gv = typeof(GlobalVars);

            foreach (var option in allNewOptions)
            {
                var optionExists = GetOption(option.ID);
                if (optionExists != null)
                {
                    //this here is some hodgepodge. we should really fix it later.
                    if (optionExists.ID == "NavigationChoices")
                    {
                        PropertyInfo prop = gv.GetRuntimeProperty(option.ID);
                        prop.SetValue(null, JsonConvert.DeserializeObject<List<DetailChoice>>(optionExists.getStringData()), null);
                    }
                    else if (optionExists.type == "System.DateTime")
                    {
                        PropertyInfo prop = gv.GetRuntimeProperty(option.ID);
                        prop.SetValue(null, (new DateTime(optionExists.getLongData())), null);
                    }
                    else
                    {
                        PropertyInfo prop = gv.GetRuntimeProperty(option.ID);
                        prop.SetValue(null, optionExists.originalValue, null);
                    }
                }
                else
                {
                    if (option.type == "System.DateTime")
                    {
                        PropertyInfo prop = gv.GetRuntimeProperty(option.ID);
                        prop.SetValue(null, new DateTime(option.getLongData()), null);
                    }
                    else
                    {
                        PropertyInfo prop = gv.GetRuntimeProperty(option.ID);
                        prop.SetValue(null, option.originalValue, null);
                    }
                }
            }

            GlobalVars.eventCount = InternalEventCount();
        }
        
        public GlobalOption GetOption(string ID)
        {
            GlobalOption returnMe = null;
            try
            {
                returnMe = database.Table<GlobalOption>().Where(d => d.ID == ID).FirstOrDefaultAsync().Result;
            }
            catch (Exception)
            {

            }
            return returnMe;
        }
                
        public async Task SetOption(GlobalOption value)
        {
         
            await database.InsertOrReplaceAsync(value);
        }

        public List<UserEventList> UserEventLists
        {
            get
            {
                return database.Table<UserEventList>().OrderBy(d => d.Title).ToListAsync().Result;
            }
        }

        public async Task<UserEventList> UpdateUserEventList(UserEventList list, bool withChildren = false)
        {
            bool nameExists = true;
            string newTitle = list.Title;

            var countAdd = 0;

            while (nameExists)
            {
                nameExists = await database.Table<UserEventList>().Where(d => d.Title.ToLower() == newTitle.ToLower() && d.ID != list.ID).CountAsync() > 0;
                if (nameExists)
                {
                    countAdd++;
                    newTitle = list.Title + $" ({countAdd})";
                }
            }

            list.Title = newTitle;

            if (withChildren)
            {
                await database.InsertOrReplaceWithChildrenAsync(list);
            }
            else
            {
                await database.InsertOrReplaceAsync(list);
            }
            return list;
        }

        public async Task<UserEventList> UpdateUserEventListWithChildrenAsync(UserEventList list)
        {
            return await UpdateUserEventList(list, true);
        }

        public async Task UpdateGenEventWithChildrenAsync(GenEvent genEvent)
        {
            await database.InsertOrReplaceWithChildrenAsync(genEvent);
        }

        public async Task UpdateGenEventListWithChildrenAsync(List<GenEvent> genEvents)
        {
            await database.InsertOrReplaceAllWithChildrenAsync(genEvents);
        }

        //public async Task<UserEventList> GetUserEventList(int ID)
        //{
        //    UserEventList returnMe = null;
        //    try
        //    {
        //        returnMe = await database.Table<UserEventList>().Where(d => d.ID == ID).FirstOrDefaultAsync();
        //    }
        //    catch (Exception)
        //    {

        //    }
        //    return returnMe;
        //}

        public async Task<UserEventList> AddUserEventList(string title)
        {
            bool nameExists = true;
            string newTitle = title;

            var countAdd = 0;
            
            while (nameExists)
            {
                nameExists = await database.Table<UserEventList>().Where(d => d.Title.ToLower() == newTitle.ToLower()).CountAsync() > 0;
                if (nameExists)
                {
                    countAdd++;
                    newTitle = title + $" ({countAdd})";
                }
            }

            UserEventList newList = new UserEventList()
            {
                Title = newTitle
            };
            
            await database.InsertAsync(newList);
            return newList;
        }

        public async Task SetUserEventList(UserEventList value)
        {
            await database.InsertOrReplaceAsync(value);
        }

        public async Task DeleteUserEventList(UserEventList value)
        {
            await database.DeleteAsync(value);
        }

        public async Task DeleteUserEventList(int ID)
        {
            var deleteMe = await database.Table<UserEventList>().Where(d => d.ID == ID).FirstOrDefaultAsync();
            if (deleteMe != null)
            {
                await database.DeleteAsync(deleteMe);
            }
        }

        //public async Task<IList<GenEvent>> GetGenEvents(List<string> IDs)
        //{
        //    var returnMe = await database.Table<GenEvent>().Where(d => IDs.Contains(d.ID)).OrderBy(d => d.StartDateTime).ToListAsync();

        //    return returnMe;
        //}

        public async Task<UserEventList> GetUserEventListWithChildrenAsync(UserEventList list)
        {
            return await database.GetWithChildrenAsync<UserEventList>(list.ID);
        }

        public async Task<IList<GenEvent>> GetItemsFTS4(string search, params DBOptions[] options)
        {
            IList<GenEvent> returnMe = null;

            bool searchExists = !string.IsNullOrEmpty(search);
            bool optionExists = options.Length > 0;
            string andOpt = "";
            bool whereAdded = false;

            List<object> args = new List<object>();

            StringBuilder query = new StringBuilder(@"SELECT * FROM GenEvent ");

            if (searchExists)
            {
                if (!whereAdded)
                {
                    whereAdded = true;
                    query.Append(" WHERE ");
                }
                query.Append(andOpt).Append("GenEvent MATCH ?");
                args.Add("Title: \"" + search + "*\"" + " OR Description: \"" + search + "*\"" + " OR ID: \"" + search + "*\"");

                andOpt = " AND ";
            } //if searchExists

            if (optionExists)
            {
                var Wednesday = GlobalVars.startingDate;
                var Thursday = GlobalVars.startingDate.AddDays(1);
                var Friday = GlobalVars.startingDate.AddDays(2);
                var Saturday = GlobalVars.startingDate.AddDays(3);
                var Sunday = GlobalVars.startingDate.AddDays(4);
                var Monday = GlobalVars.startingDate.AddDays(5);

                TimeSpan afterTime = new TimeSpan(0);
                TimeSpan beforeTime = new TimeSpan(24, 0, 0);

                string sortOrder = "ASC";
                string overrideSortOrder = "";

                foreach (var option in options)
                {
                    switch (option)
                    {
                        case DBOptions.After00:
                            afterTime = new TimeSpan(0, 0, 0);
                            break;
                        case DBOptions.After01:
                            afterTime = new TimeSpan(1, 0, 0);
                            break;
                        case DBOptions.After02:
                            afterTime = new TimeSpan(2, 0, 0);
                            break;
                        case DBOptions.After03:
                            afterTime = new TimeSpan(3, 0, 0);
                            break;
                        case DBOptions.After04:
                            afterTime = new TimeSpan(4, 0, 0);
                            break;
                        case DBOptions.After05:
                            afterTime = new TimeSpan(5, 0, 0);
                            break;
                        case DBOptions.After06:
                            afterTime = new TimeSpan(6, 0, 0);
                            break;
                        case DBOptions.After07:
                            afterTime = new TimeSpan(7, 0, 0);
                            break;
                        case DBOptions.After08:
                            afterTime = new TimeSpan(8, 0, 0);
                            break;
                        case DBOptions.After09:
                            afterTime = new TimeSpan(9, 0, 0);
                            break;
                        case DBOptions.After10:
                            afterTime = new TimeSpan(10, 0, 0);
                            break;
                        case DBOptions.After11:
                            afterTime = new TimeSpan(11, 0, 0);
                            break;
                        case DBOptions.After12:
                            afterTime = new TimeSpan(12, 0, 0);
                            break;
                        case DBOptions.After13:
                            afterTime = new TimeSpan(13, 0, 0);
                            break;
                        case DBOptions.After14:
                            afterTime = new TimeSpan(14, 0, 0);
                            break;
                        case DBOptions.After15:
                            afterTime = new TimeSpan(15, 0, 0);
                            break;
                        case DBOptions.After16:
                            afterTime = new TimeSpan(16, 0, 0);
                            break;
                        case DBOptions.After17:
                            afterTime = new TimeSpan(17, 0, 0);
                            break;
                        case DBOptions.After18:
                            afterTime = new TimeSpan(18, 0, 0);
                            break;
                        case DBOptions.After19:
                            afterTime = new TimeSpan(19, 0, 0);
                            break;
                        case DBOptions.After20:
                            afterTime = new TimeSpan(20, 0, 0);
                            break;
                        case DBOptions.After21:
                            afterTime = new TimeSpan(21, 0, 0);
                            break;
                        case DBOptions.After22:
                            afterTime = new TimeSpan(22, 0, 0);
                            break;
                        case DBOptions.After23:
                            afterTime = new TimeSpan(23, 0, 0);
                            break;
                        case DBOptions.Before01:
                            beforeTime = new TimeSpan(1, 0, 0);
                            break;
                        case DBOptions.Before02:
                            beforeTime = new TimeSpan(2, 0, 0);
                            break;
                        case DBOptions.Before03:
                            beforeTime = new TimeSpan(3, 0, 0);
                            break;
                        case DBOptions.Before04:
                            beforeTime = new TimeSpan(4, 0, 0);
                            break;
                        case DBOptions.Before05:
                            beforeTime = new TimeSpan(5, 0, 0);
                            break;
                        case DBOptions.Before06:
                            beforeTime = new TimeSpan(6, 0, 0);
                            break;
                        case DBOptions.Before07:
                            beforeTime = new TimeSpan(7, 0, 0);
                            break;
                        case DBOptions.Before08:
                            beforeTime = new TimeSpan(8, 0, 0);
                            break;
                        case DBOptions.Before09:
                            beforeTime = new TimeSpan(9, 0, 0);
                            break;
                        case DBOptions.Before10:
                            beforeTime = new TimeSpan(10, 0, 0);
                            break;
                        case DBOptions.Before11:
                            beforeTime = new TimeSpan(11, 0, 0);
                            break;
                        case DBOptions.Before12:
                            beforeTime = new TimeSpan(12, 0, 0);
                            break;
                        case DBOptions.Before13:
                            beforeTime = new TimeSpan(13, 0, 0);
                            break;
                        case DBOptions.Before14:
                            beforeTime = new TimeSpan(14, 0, 0);
                            break;
                        case DBOptions.Before15:
                            beforeTime = new TimeSpan(15, 0, 0);
                            break;
                        case DBOptions.Before16:
                            beforeTime = new TimeSpan(16, 0, 0);
                            break;
                        case DBOptions.Before17:
                            beforeTime = new TimeSpan(17, 0, 0);
                            break;
                        case DBOptions.Before18:
                            beforeTime = new TimeSpan(18, 0, 0);
                            break;
                        case DBOptions.Before19:
                            beforeTime = new TimeSpan(19, 0, 0);
                            break;
                        case DBOptions.Before20:
                            beforeTime = new TimeSpan(20, 0, 0);
                            break;
                        case DBOptions.Before21:
                            beforeTime = new TimeSpan(21, 0, 0);
                            break;
                        case DBOptions.Before22:
                            beforeTime = new TimeSpan(22, 0, 0);
                            break;
                        case DBOptions.Before23:
                            beforeTime = new TimeSpan(23, 0, 0);
                            break;
                        case DBOptions.Before24:
                            beforeTime = new TimeSpan(24, 0, 0);
                            break;
                    }
                }

                foreach (var option in options)
                { 
                    switch (option)
                    {
                        case DBOptions.FilterWednesday:
                            if (!whereAdded)
                            {
                                whereAdded = true;
                                query.Append(" WHERE ");
                            }
                            query.Append(andOpt).Append("(StartDateTime >= ? AND StartDateTime < ?)");
                            args.Add(Wednesday.Add(afterTime).Ticks);
                            args.Add(Wednesday.Add(beforeTime).Ticks);
                                
                            andOpt = " AND ";
                            break;
                        case DBOptions.FilterThursday:
                            if (!whereAdded)
                            {
                                whereAdded = true;
                                query.Append(" WHERE ");
                            }
                            query.Append(andOpt).Append("(StartDateTime >= ? AND StartDateTime < ?)");
                            args.Add(Thursday.Add(afterTime).Ticks);
                            args.Add(Thursday.Add(beforeTime).Ticks);
                            andOpt = " AND ";
                            break;
                        case DBOptions.FilterFriday:
                            if (!whereAdded)
                            {
                                whereAdded = true;
                                query.Append(" WHERE ");
                            }
                            query.Append(andOpt).Append("(StartDateTime >= ? AND StartDateTime < ?)");
                            args.Add(Friday.Add(afterTime).Ticks);
                            args.Add(Friday.Add(beforeTime).Ticks);
                            andOpt = " AND ";
                            break;
                        case DBOptions.FilterSaturday:
                            if (!whereAdded)
                            {
                                whereAdded = true;
                                query.Append(" WHERE ");
                            }
                            query.Append(andOpt).Append("(StartDateTime >= ? AND StartDateTime < ?)");
                            args.Add(Saturday.Add(afterTime).Ticks);
                            args.Add(Saturday.Add(beforeTime).Ticks);
                            andOpt = " AND ";
                            break;
                        case DBOptions.FilterSunday:
                            if (!whereAdded)
                            {
                                whereAdded = true;
                                query.Append(" WHERE ");
                            }
                            query.Append(andOpt).Append("(StartDateTime >= ? AND StartDateTime <= ?)");
                            args.Add(Sunday.Add(afterTime).Ticks);
                            args.Add(Sunday.Add(beforeTime).Ticks);
                            andOpt = " AND ";
                            break;

                        case DBOptions.FilterAllDays:
                            if (!whereAdded)
                            {
                                whereAdded = true;
                                query.Append(" WHERE ");
                            }
                            query.Append(andOpt).Append("((StartDateTime >= ? AND StartDateTime <= ?) OR ");
                            args.Add(Wednesday.Add(afterTime).Ticks);
                            args.Add(Wednesday.Add(beforeTime).Ticks);

                            query.Append("(StartDateTime >= ? AND StartDateTime <= ?) OR ");
                            args.Add(Thursday.Add(afterTime).Ticks);
                            args.Add(Thursday.Add(beforeTime).Ticks);

                            query.Append("(StartDateTime >= ? AND StartDateTime <= ?) OR ");
                            args.Add(Friday.Add(afterTime).Ticks);
                            args.Add(Friday.Add(beforeTime).Ticks);

                            query.Append("(StartDateTime >= ? AND StartDateTime <= ?) OR ");
                            args.Add(Saturday.Add(afterTime).Ticks);
                            args.Add(Saturday.Add(beforeTime).Ticks);

                            query.Append("(StartDateTime >= ? AND StartDateTime <= ?))");
                            args.Add(Sunday.Add(afterTime).Ticks);
                            args.Add(Sunday.Add(beforeTime).Ticks);
                            break;
                        default:
                            break;
                    }

                    switch (option)
                    {
                        case DBOptions.SortPrice:
                            sortOrder = "ASC";
                            query.Append(" ORDER BY parsedCost {sortOrder}");
                            break;
                        case DBOptions.SortTickets:
                            sortOrder = "DESC";
                            query.Append(" ORDER BY AvailableTickets {sortOrder}");
                            break;
                        case DBOptions.SortTime:
                            sortOrder = "ASC";
                            query.Append(" ORDER BY StartDateTime {sortOrder}");
                            break;
                        case DBOptions.SortTitle:
                            sortOrder = "ASC";
                            query.Append(" ORDER BY Title {sortOrder}");
                            break;
                        case DBOptions.SortAscending:
                            overrideSortOrder = "ASC";
                            break;
                        case DBOptions.SortDescending:
                            overrideSortOrder = "DESC";
                            break;
                        default:
                            break;
                    }
                }

                query.Replace("{sortOrder}", string.IsNullOrEmpty(overrideSortOrder) ? sortOrder : overrideSortOrder);

            } //if optionExists

            returnMe = await database.QueryAsync<GenEvent>(query.ToString(), args.ToArray());

            return returnMe;
        }
        
        public class table_info_record
        {
            public int cid { get; set; }
            public string name { get; set; }
            public string type { get; set; }
            public int notnull { get; set; }
            public string dflt_value { get; set; }
            public int pk { get; set; }
        }

        public void SaveItems(List<GenEvent> items)
        {

            List<table_info_record> columnNameQueryResult = new List<table_info_record>();

            try
            {
                columnNameQueryResult = database.QueryAsync<table_info_record>("PRAGMA table_info(GenEvent);").Result;
            }
            catch (Exception)
            {

            }

            List<string> columnNames = new List<string>();
            columnNames.Add("docid");
            columnNameQueryResult.ForEach(d => columnNames.Add(d.name));

            string insertStub = "INSERT OR REPLACE INTO GenEvent (";

            for (var c = 0; c < columnNames.Count; c++)
            {
                if (c != columnNames.Count - 1)
                {
                    insertStub += columnNames[c] + ", ";
                }
                else
                {
                    insertStub += columnNames[c];
                }
            }

            insertStub += ") values (";

            for (var c = 0; c < columnNames.Count; c++)
            {
                if (c != columnNames.Count - 1)
                {
                    insertStub += "?, ";
                }
                else
                {
                    insertStub += "?";
                }
            }

            insertStub += ")";

            var args = new object[columnNames.Count];
            List<PropertyInfo> reflectionProperties = new List<PropertyInfo>();
            if (items.Count > 0)
            {
                for (var c = 0; c < columnNames.Count; c++)
                {
                    reflectionProperties.Add(items[0].GetType().GetRuntimeProperties().FirstOrDefault(d => string.Equals(d.Name, columnNames[c], StringComparison.OrdinalIgnoreCase)));
                }
            }

            try
            {
                database.RunInTransactionAsync(delegate (SQLiteConnection transaction)
                {
                    foreach (var item in items)
                    {
                        for (var c = 0; c < columnNames.Count; c++)
                        {
                            if (columnNames[c] == "Title")
                            {
                                args[c] = ((string)(reflectionProperties[c]?.GetValue(item, null))).Trim();
                            }
                            else
                            {
                                args[c] = reflectionProperties[c]?.GetValue(item, null);
                            }

                            if (args[c].GetType() == typeof(System.DateTime))
                            {
                                args[c] = (long)(((DateTime)args[c]).Ticks);
                            }
                        }

                        transaction.Execute(insertStub, args);
                    }
                }).Wait();
            }
            catch (Exception)
            {

            }

            //dog slow, don't bother. like millions of times slower.
            //foreach(var item in items)
            //{
            //    rowsAffected = await database.UpdateAsync(item);
            //    if (rowsAffected == 0)
            //    {
            //        await database.InsertAsync(item);
            //    }
            //}

            GlobalVars.eventCount = InternalEventCount();
            //}
        }

        /// <summary>
        /// Saves the given GenEvents to database.
        /// </summary>
        /// <param name="items"></param>
        /// <returns>A phrase designed to be placed into a yellow popup if notemptyornull</returns>
        public async Task<string> SaveItemsAsync(List<GenEvent> items)
        {
            bool LoggedUpdatesWereFoundInLists = false;

            List<string> oldListEventIDs = new List<string>();

            // get all user list events
            var userLists = await database.Table<UserEventList>().Where(d => d.ID > -1).ToListAsync();
            foreach (var userList in userLists)
            {
                //var userListWithChildren = await database.GetWithChildrenAsync<UserEventList>(userList);
                var userListChildrenEvents = await database.Table<GenEventUserEventList>().Where(d => d.UserEventListID == userList.ID).ToListAsync();
                foreach (var foundEvent in userListChildrenEvents)
                {
                    if (items.FirstOrDefault(d => d.ID == foundEvent.GenEventID) != null)
                    {
                        oldListEventIDs.Add(foundEvent.GenEventID);
                    }
                }
            }

            oldListEventIDs = oldListEventIDs.Distinct().ToList();

            List<GenEvent> oldEvents = new List<GenEvent>();

            if (oldListEventIDs.Count > 0)
            {
                oldEvents = await database.Table<GenEvent>().Where(d => oldListEventIDs.Contains(d.ID)).ToListAsync();
            }

            List<EventChangeLog> newChangeLogs = new List<EventChangeLog>();

            DateTime changeTime = DateTime.Now;

            foreach (var oldEventID in oldListEventIDs)
            {
                var newEvent = items.FirstOrDefault(d => d.ID == oldEventID);
                if (newEvent != null)
                {
                    var oldEvent = oldEvents.FirstOrDefault(d => d.ID == oldEventID);
                    if (oldEvent != null)
                    {
                        var detectedChanges = GenEventHelpers.GetChanges(oldEvent, newEvent, changeTime);
                        if (detectedChanges.Count > 0)
                        {
                            //oldEvent.ChangeLogs.AddRange(detectedChanges);
                            newChangeLogs.AddRange(detectedChanges);
                        }
                    }
                }
            }

            if (newChangeLogs.Count > 0)
            {
                await database.InsertAllAsync(newChangeLogs);
                LoggedUpdatesWereFoundInLists = true;
            }

            // start of manual insert-or-replace of items (does not affect many-many or one-many relationships)
            var columnNameQueryResult = await database.QueryAsync<table_info_record>("PRAGMA table_info(GenEvent);");
            List<string> columnNames = new List<string>();
            columnNames.Add("docid");
            columnNameQueryResult.ForEach(d => columnNames.Add(d.name));

            string insertStub = "INSERT OR REPLACE INTO GenEvent (";

            for (var c = 0; c < columnNames.Count; c++)
            {
                if (c != columnNames.Count - 1)
                {
                    insertStub += columnNames[c] + ", ";
                }
                else
                {
                    insertStub += columnNames[c];
                }
            }
                
            insertStub += ") values (";

            for (var c = 0; c < columnNames.Count; c++)
            {
                if (c != columnNames.Count - 1)
                {
                    insertStub += "?, ";
                }
                else
                {
                    insertStub += "?";
                }
            }

            insertStub += ")";

            var args = new object[columnNames.Count];
            List<PropertyInfo> reflectionProperties = new List<PropertyInfo>();
            if (items.Count > 0)
            {
                for (var c = 0; c < columnNames.Count; c++)
                {
                    reflectionProperties.Add(items[0].GetType().GetRuntimeProperties().FirstOrDefault(d => string.Equals(d.Name, columnNames[c], StringComparison.OrdinalIgnoreCase)));
                }
            }

            await database.RunInTransactionAsync(delegate (SQLiteConnection transaction) {
                foreach (var item in items)
                {
                    for (var c = 0; c < columnNames.Count; c++)
                    {
                        if (columnNames[c] == "Title")
                        {
                            args[c] = ((string)(reflectionProperties[c]?.GetValue(item, null))).Trim();
                        }
                        else
                        {
                            args[c] = reflectionProperties[c]?.GetValue(item, null);
                        }

                        if (args[c].GetType() == typeof(System.DateTime))
                        {
                            args[c] = (long)(((DateTime)args[c]).Ticks);
                        }
                    }

                    transaction.Execute(insertStub, args);
                }
            });

            if (LoggedUpdatesWereFoundInLists)
            {
                List<string> finalUpdatedIDs = newChangeLogs.Select(d => d.GenEventID).Distinct().ToList();

                var notificationUpdateEvents = await database.Table<GenEvent>().Where(d => finalUpdatedIDs.Contains(d.ID)).ToListAsync();
                foreach (var notifyEvent in notificationUpdateEvents)
                {
                    notifyEvent.HasUpdateNotifications = true;
                    await database.UpdateAsync(notifyEvent);
                }
            }
            

            //dog slow, don't bother. like millions of times slower.
            //foreach(var item in items)
            //{
            //    rowsAffected = await database.UpdateAsync(item);
            //    if (rowsAffected == 0)
            //    {
            //        await database.InsertAsync(item);
            //    }
            //}

            GlobalVars.eventCount = await InternalEventCountAsync();

            var returnMe = "";

            if (LoggedUpdatesWereFoundInLists)
            {
                returnMe = "Some events in your saved lists were updated! They have been marked in yellow.";
            }

            ((App)Application.Current).homePage.userListPage.IsUpdateRequested = true;

            return returnMe;
        }

        public async Task<GenEvent> GetGenEventAsync(string ID)
        {
            return await database.Table<GenEvent>().Where(d => d.ID == ID).FirstOrDefaultAsync();
        }

        public async Task DeleteAllEventChangeLogsAsync(List<EventChangeLog> toDelete)
        {
            for (int c = 0; c < toDelete.Count; c++)
            {
                await database.DeleteAsync(toDelete[c]);
            }
            // await database.DeleteAllAsync(toDelete);
        }

        public void DeleteAllEventChangeLogs(List<EventChangeLog> toDelete)
        {
            database.DeleteAllAsync(toDelete).Wait();
        }

        public void SetGenEventUpdateNotificationAsRead(GenEvent genEvent)
        {
            genEvent.HasUpdateNotifications = false;
            database.UpdateAsync(genEvent).Wait();
        }

        public async Task SetGenEventUpdateNotificationAsReadAsync(GenEvent genEvent)
        {
            genEvent.HasUpdateNotifications = false;
            await database.UpdateAsync(genEvent);
        }

        public async Task<List<EventChangeLog>> GetEventChangeLogsForGenEvent(GenEvent genEvent)
        {
            return await database.Table<EventChangeLog>().Where(d => d.GenEventID == genEvent.ID).ToListAsync();
        }

        public int InternalEventCount()
        {
            int returnMe = -1;
            try
            {
                returnMe = database.Table<GenEvent>().CountAsync().Result;
            }
            catch (Exception)
            {

            }
            return returnMe;
        }
        
        public async Task<int> InternalEventCountAsync()
        {
            return await database.Table<GenEvent>().CountAsync();
        }
              
        public async Task<DateTime> GetLastSyncTime()
        {
            var eventCount = await InternalEventCountAsync();

            DateTime returnMe;

            if (eventCount > 0)
            {
                var temp = await database.Table<GenEvent>().OrderByDescending(d => d.SyncTime).FirstOrDefaultAsync();
                returnMe = temp.SyncTime;
            }
            else
            {
                returnMe = GlobalVars.yearlyStartingDate;
            }

            return returnMe;
        }
    }
}
