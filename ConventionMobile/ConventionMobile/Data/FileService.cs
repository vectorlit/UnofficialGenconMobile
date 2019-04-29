using MoreLinq;
using ModernHttpClient;
using System.Net.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ConventionMobile.Data
{
    public class FileService : IFileService
    {
        public event EventHandler FileDownloadProgressUpdated = delegate { };

        protected virtual void OnFileDownloadProgressUpdated(FileDownloadUpdateEventArgs e)
        {
            FileDownloadProgressUpdated?.Invoke(this, e);
        }

        public async Task<List<bool>> DownloadFiles(List<string> fileURL, List<string> fileNames)
        {
            List<bool> returnMe = new List<bool>(fileURL.Count);
            
            if (fileURL.Count == fileNames.Count)
            {
                for (var c = 0; c < fileURL.Count; c++)
                {
                    try
                    {
                        returnMe.Add(false);
                        //HttpClient client = new HttpClient(new NativeMessageHandler()
                        //{
                        //    Timeout = new TimeSpan(0, 0, 9),
                        //    EnableUntrustedCertificates = true,
                        //    DisableCaching = true
                        //});
                        HttpClient client = new HttpClient();
                        client.MaxResponseContentBufferSize = 25600000;
                        var response = await client.GetAsync(fileURL[c]).ConfigureAwait(false);

                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsByteArrayAsync();
                            //var webHeader = response.Content.Headers.GetValues("Content-Disposition")?.FirstOrDefault();
                            //if (string.IsNullOrEmpty(webHeader))
                            //{
                            //    webHeader = "DownloadFile" + c.ToString();
                            //}
                            //var fileName = DependencyService.Get<IFileOps>().GetFileLocation(!string.IsNullOrEmpty(fileNames[c]) ? fileNames[c] : webHeader);

                            var fileName = DependencyService.Get<IFileOps>().GetFileLocation(fileNames[c]);

                            DependencyService.Get<IFileOps>().SaveFile(content, fileName);

                            OnFileDownloadProgressUpdated(new FileDownloadUpdateEventArgs(c + 1, fileURL.Count));

                            returnMe[c] = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        returnMe[c] = false;
                        string message = ex.Message;
                    }
                }
            }
            return returnMe;
        }
    }
}
