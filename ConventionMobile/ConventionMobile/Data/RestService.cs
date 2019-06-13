using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConventionMobile.Model;
using ModernHttpClient;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http;
using System.Dynamic;

namespace ConventionMobile.Data
{
    public class RestService : IRestService
    {
        //HttpClient client = null;

        public List<GenEvent> events { get; private set; }

        public event EventHandler TotalEventsCalculated = delegate { };
        public event EventHandler EventsDownloadProgressCalculated = delegate { };

        private const int indexSize = 250;


        public RestService()
        {
            //client = new HttpClient(new NativeMessageHandler());
            //LOOK HERE IF THERE IS A PROBLEM WITH DATA CORRUPT AND NOT RETURNING
            //client.MaxResponseContentBufferSize = 25600000;
        }

        protected virtual void OnTotalEventsCalculated(DownloadUpdateEventArgs e)
        {
            TotalEventsCalculated?.Invoke(this, e);
        }

        protected virtual void OnEventsDownloadProgressCalculated(DownloadUpdateEventArgs e)
        {
            EventsDownloadProgressCalculated?.Invoke(this, e);
        }

        public async Task<List<GenEvent>> RefreshAllAsync(DateTime? lastSyncTime = null)
        {
            events = new List<GenEvent>();

            int downloadedCount = 0;

            var totalNumberOfEvents = await GetNumberOfEvents(lastSyncTime);

            bool requestStop = false;

            DateTime lastEventTime = lastSyncTime == null ? GlobalVars.yearlyStartingDate : (DateTime)lastSyncTime;

            int skipNum = 0;
            int iterationCount = 0;

            OnTotalEventsCalculated(new DownloadUpdateEventArgs(totalNumberOfEvents));

            while (events.Count < totalNumberOfEvents && !requestStop)
            {
                var uri = new Uri(String.Format(GlobalVars.GenEventAllEventsCustomizableURL + "/{1}/{2}", lastEventTime.ToString("yyyy-MM-dd't'HH:mm:ss"), indexSize, skipNum));
                downloadedCount = events.Count;

                try
                {
                    //using (var client = new HttpClient(new NativeMessageHandler()
                    //{
                    //    Timeout = new TimeSpan(0, 0, 9),
                    //    EnableUntrustedCertificates = true,
                    //    DisableCaching = true
                    //}))
                    using (var client = new HttpClient())
                    {
                        client.MaxResponseContentBufferSize = 25600000;

                        var response = await client.GetAsync(uri).ConfigureAwait(false);
                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                            events.AddRange(JsonConvert.DeserializeObject<List<GenEvent>>(content));

                            downloadedCount = events.Count - downloadedCount;

                            iterationCount++;
                            skipNum = indexSize * iterationCount;

                            if (events.Count >= totalNumberOfEvents || skipNum > totalNumberOfEvents || downloadedCount < indexSize)
                            {
                                requestStop = true;
                            }

                        }
                        else
                        {
                            requestStop = true;
                        }
                    }

                    OnEventsDownloadProgressCalculated(new DownloadUpdateEventArgs(events.Count));

                    if (events.Count >= totalNumberOfEvents)
                    {
                        requestStop = true;
                    }                    
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(@"     ERROR {0}: {1}", DateTime.Now.ToString(), ex.Message);
                }
            }

            return events;
        }

        //public async Task<List<GenEvent>> RefreshNewAsync(DateTime lastSyncTime)
        //{
        //    events = new List<GenEvent>();

        //    var uri = new Uri(GlobalVars.GenEventAfterDateURL(lastSyncTime));

        //    try
        //    {
        //        var response = await client.GetAsync(uri).ConfigureAwait(false);
        //        if (response.IsSuccessStatusCode)
        //        {
        //            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        //            events = JsonConvert.DeserializeObject<List<GenEvent>>(content);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine(@"     ERROR {0}: {1}", DateTime.Now.ToString(), ex.Message);
        //    }

        //    return events;
        //}

        public async Task<int> GetNumberOfEvents(DateTime? lastSyncTime = null)
        {
            int numEvents = -1;

            var uri = new Uri(GlobalVars.GenEventAfterDateEventsCountURL(lastSyncTime));

            try
            {
                //using (var client = new HttpClient(new NativeMessageHandler()
                //{
                //    Timeout = new TimeSpan(0, 0, 9),
                //    EnableUntrustedCertificates = true,
                //    DisableCaching = true
                //}))
                using (var client = new HttpClient())
                {
                    client.MaxResponseContentBufferSize = 25600000;

                    var response = await client.GetAsync(uri).ConfigureAwait(false);
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        numEvents = JsonConvert.DeserializeObject<int>(content);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"     ERROR {0}: {1}", DateTime.Now.ToString(), ex.Message);
            }

            return numEvents;
        }

        //public async Task<int> GetNumberOfEventsAfterSyncTime(DateTime lastSyncTime)
        //{
        //    var numEvents = -1;

        //    var uri = new Uri(GlobalVars.GenEventAfterDateURL(lastSyncTime, true));

        //    try
        //    {
        //        //using (var client = new HttpClient(new NativeMessageHandler()
        //        //{
        //        //    Timeout = new TimeSpan(0, 0, 9),
        //        //    EnableUntrustedCertificates = true,
        //        //    DisableCaching = true
        //        //}))
        //        using (var client = new HttpClient())
        //        {
        //            client.MaxResponseContentBufferSize = 25600000;

        //            var response = await client.GetAsync(uri).ConfigureAwait(false);
        //            if (response.IsSuccessStatusCode)
        //            {
        //                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        //                numEvents = JsonConvert.DeserializeObject<int>(content);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine(@"     ERROR {0}: {1}", DateTime.Now.ToString(), ex.Message);
        //    }

        //    return numEvents;
        //}

        public class UserEventListViewModel
        {
            public string Title { get; set; }

            public List<string> Events { get; set; }

            public string ExternalAddress { get; set; }
            public string InternalSecret { get; set; }

        }

        public class UserEventListReturnModel
        {
            public string ExternalAddress { get; set; }
            public string InternalSecret { get; set; }

        }

        public async Task<UserEventList> InsertUpdateUserEventListAsync(UserEventList submitMe)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = new TimeSpan(0, 0, 15);

                    //client.BaseAddress = new Uri(GlobalVars.UserEventListURL);
                    var viewModelSubmit = new UserEventListViewModel
                    {
                        Title = submitMe.Title,
                        Events = submitMe.Events.Select(d => d.ID).ToList(),
                        ExternalAddress = submitMe.ExternalAddress,
                        InternalSecret = submitMe.InternalSecret
                    };

                    var serializedObject = JsonConvert.SerializeObject(viewModelSubmit);

                    HttpContent content = new StringContent(serializedObject, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(GlobalVars.UserEventListURL + "/u", content);
                    var responseStringContents = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(responseStringContents))
                    {
                        try
                        {
                            var responseUserEventListReturnModel = JsonConvert.DeserializeObject<UserEventListReturnModel>(responseStringContents);
                            var responseUserEventList = new UserEventList
                            {
                                ID = submitMe.ID,
                                Events = submitMe.Events,
                                ExternalAddress = responseUserEventListReturnModel.ExternalAddress,
                                InternalSecret = responseUserEventListReturnModel.InternalSecret,
                                Title = submitMe.Title,
                                HasEventListChangedSinceSync = false
                            };
                            return responseUserEventList;
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return null;
        }

        /// <summary>
        /// Returns a test set of 100 items
        /// </summary>
        /// <returns></returns>
        public async Task<List<GenEvent>> Test100()
        {
            events = new List<GenEvent>();

            var uri = new Uri(GlobalVars.GenEventURL);

            try
            {
                //using (var client = new HttpClient(new NativeMessageHandler()
                //{
                //    Timeout = new TimeSpan(0, 0, 9),
                //    EnableUntrustedCertificates = true,
                //    DisableCaching = true
                //}))
                using (var client = new HttpClient())
                {
                    client.MaxResponseContentBufferSize = 25600000;

                    var response = await client.GetAsync(uri).ConfigureAwait(false);
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        events = JsonConvert.DeserializeObject<List<GenEvent>>(content);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"     ERROR {0}: {1}", DateTime.Now.ToString(), ex.Message);
            }

            return events;
        }
        
    }
}
