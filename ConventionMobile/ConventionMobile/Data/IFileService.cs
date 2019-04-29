using ConventionMobile.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConventionMobile.Data
{
    public interface IFileService
    {
        event EventHandler FileDownloadProgressUpdated;

        Task<List<bool>> DownloadFiles(List<string> fileURL, List<string> fileName);
    }

    public class FileDownloadUpdateEventArgs : EventArgs
    {
        public double totalAmount { get; set; }
        public double currentAmount { get; set; }

        public FileDownloadUpdateEventArgs()
        {
            totalAmount = 0;
            currentAmount = 0;
        }

        public FileDownloadUpdateEventArgs(double currentAmount, double totalAmount)
        {
            this.totalAmount = totalAmount;
            this.currentAmount = currentAmount;
        }
    }
}
