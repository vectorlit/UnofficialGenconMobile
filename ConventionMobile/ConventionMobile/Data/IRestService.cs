using ConventionMobile.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConventionMobile.Data
{
    public interface IRestService
    {
        event EventHandler TotalEventsCalculated;
        event EventHandler EventsDownloadProgressCalculated;

        Task<List<GenEvent>> RefreshAllAsync(DateTime? lastSyncTime);
        //Task<List<GenEvent>> RefreshNewAsync(DateTime lastSyncTime);
        Task<List<GenEvent>> Test100();

        Task<int> GetNumberOfEvents();
        Task<int> GetNumberOfEventsAfterSyncTime(DateTime lastSyncTime);
        Task<UserEventList> InsertUpdateUserEventListAsync(UserEventList submitList);
    }

    public class DownloadUpdateEventArgs : EventArgs
    {
        public double number { get; set; }

        public DownloadUpdateEventArgs()
        {
            number = 0;
        }

        public DownloadUpdateEventArgs(double number)
        {
            this.number = number;
        }
    }
}
