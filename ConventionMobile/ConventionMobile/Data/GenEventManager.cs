using ConventionMobile.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConventionMobile.Data
{
    public class GenEventManager
    {
        IRestService restService = null;

        public event EventHandler TotalEventsCalculated = delegate { };
        public event EventHandler EventsDownloadProgressCalculated = delegate { };

        public GenEventManager(IRestService service)
        {
            restService = service;

            restService.TotalEventsCalculated += RestService_TotalEventsCalculated;
            restService.EventsDownloadProgressCalculated += RestService_EventsDownloadProgressCalculated;
        }

        private void RestService_EventsDownloadProgressCalculated(object sender, EventArgs e)
        {
            OnEventsDownloadProgressCalculated((DownloadUpdateEventArgs)e);
        }

        private void RestService_TotalEventsCalculated(object sender, EventArgs e)
        {
            OnTotalEventsCalculated((DownloadUpdateEventArgs)e);
        }

        public Task<List<GenEvent>> GetAllEventsAsync()
        {
            return restService.RefreshAllAsync(null);
        }

        public Task<List<GenEvent>> GetEventsAsync(DateTime lastSyncTime)
        {
            return restService.RefreshAllAsync(lastSyncTime);
        }

        public async Task<UserEventList> InsertUpdateUserEventListAsync(UserEventList submitList)
        {
            return await restService.InsertUpdateUserEventListAsync(submitList);
        }

        public Task<List<GenEvent>> Test100()
        {
            return restService.Test100();
        }

        protected virtual void OnTotalEventsCalculated(DownloadUpdateEventArgs e)
        {
            TotalEventsCalculated?.Invoke(this, e);
        }

        protected virtual void OnEventsDownloadProgressCalculated(DownloadUpdateEventArgs e)
        {
            EventsDownloadProgressCalculated?.Invoke(this, e);
        }
        
    }
}
