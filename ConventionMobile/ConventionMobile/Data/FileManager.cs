using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConventionMobile.Data
{
    public class FileManager
    {
        IFileService fileService = null;
        
        public event EventHandler FileDownloadProgressUpdated = delegate { };

        public FileManager(IFileService service)
        {
            this.fileService = service;

            this.fileService.FileDownloadProgressUpdated += FileService_FileDownloadProgressUpdated;
        }

        public async Task<List<bool>> DownloadFiles(List<string> fileURL, List<string> fileNames)
        {
            return await fileService.DownloadFiles(fileURL, fileNames);
        }
        
        private void FileService_FileDownloadProgressUpdated(object sender, EventArgs e)
        {
            OnFileDownloadProgressUpdated((FileDownloadUpdateEventArgs)e);
        }

        protected virtual void OnFileDownloadProgressUpdated(FileDownloadUpdateEventArgs e)
        {
            FileDownloadProgressUpdated?.Invoke(this, e);
        }
    }
}
