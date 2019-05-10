using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ConventionMobile.Business
{
    public enum LoadingState
    {
        NotLoading = 0,
        LoadingEvents,
        LoadingMaps
    }

    public class GenConBusiness
    {
        private LoadingState _loadingState = LoadingState.NotLoading;

        public void ShowLoadingEventMessage(string message, int timeout = 1000)
        {
            if (_loadingState != LoadingState.NotLoading)
            {
                GlobalVars.DoToast(message, GlobalVars.ToastType.Red, timeout);
            }
            
        }

        public async Task CheckForNewEventsAsync()
        {
            var searchPage = (Application.Current as App)?.HomePage.GenSearchPage;
            var dontShowFurtherToasts = false;
            if (GlobalVars.isSyncNeeded && _loadingState == LoadingState.NotLoading)
            {
                _loadingState = LoadingState.LoadingEvents;

                GlobalVars.DoToast("Now checking for updated events...", GlobalVars.ToastType.Yellow);
                var events = await App.GenEventManager.GetEventsAsync(await GlobalVars.serverLastSyncTime());

                var toastText = "";

                if (events.Count > 0)
                {
                    toastText = await GlobalVars.db.SaveItemsAsync(events);
                }

                GlobalVars.lastSyncTime = DateTime.Now;

                if (!string.IsNullOrEmpty(toastText))
                {
                    GlobalVars.DoToast(toastText, GlobalVars.ToastType.Yellow, 10000);
                    dontShowFurtherToasts = true;
                }
                else
                {
                    GlobalVars.DoToast("All events are now up-to-date.", GlobalVars.ToastType.Green);
                }
                
                searchPage?.UpdateEventInfo();
                await CheckForNewGlobalVarsAsync(dontShowFurtherToasts);
            }
        }

        private async Task CheckForNewGlobalVarsAsync(bool dontShowFurtherToasts)
        {
            var searchPage = (Application.Current as App)?.HomePage.GenSearchPage;
            _loadingState = LoadingState.LoadingMaps;

            if (!dontShowFurtherToasts)
            {
                GlobalVars.DoToast("Now checking for updated maps or other info...", GlobalVars.ToastType.Yellow);
            }

            using (var client = new HttpClient())
            {
                client.MaxResponseContentBufferSize = 25600000;
                var indyTime = DependencyService.Get<ICalendar>().ConvertToIndy(GlobalVars.lastGlobalVarUpdateTime);
                var newUrl = String.Format(GlobalVars.GlobalOptionsURLCustomizableURL, indyTime.ToString("yyyy-MM-dd't'HH:mm:ss"));

                try
                {
                    var response = await client.GetAsync(newUrl).ConfigureAwait(false);

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                        if (content.Length > 10)
                        {
                            GlobalVars.FileDownloadProgressUpdated += GlobalVars_FileDownloadProgressUpdated;

                            bool totalSuccess = await GlobalVars.OverwriteOptions(content);

                            GlobalVars.FileDownloadProgressUpdated -= GlobalVars_FileDownloadProgressUpdated;

                            GlobalVars.DoToast("Update success - **REFRESHING SCREEN**", GlobalVars.ToastType.Green);
                            
                            searchPage?.UpdateEventInfo();

                            if (searchPage != null) await searchPage.CloseAllPickers();
                            Device.BeginInvokeOnMainThread(async () =>
                            {
                                try
                                {
                                    await Task.Delay(1000);
                                    (Application.Current as App)?.ShowMainPage();
                                }
                                catch (Exception)
                                {

                                }
                            });


                        }
                        else
                        {
                            if (!dontShowFurtherToasts)
                            {
                                GlobalVars.DoToast("You are up to date.", GlobalVars.ToastType.Green);
                            }

                            try
                            {
                                Device.BeginInvokeOnMainThread(() =>
                                {
                                    searchPage?.UpdateEventInfo();
                                });
                            }
                            catch (Exception)
                            {

                            }
                            GlobalVars.lastGlobalVarUpdateTime = DateTime.Now;
                        }
                    }
                }
                catch (Exception)
                {
                    if (!dontShowFurtherToasts)
                    {
                        GlobalVars.DoToast("Couldn't update now, but we'll try again later.", GlobalVars.ToastType.Yellow);
                    }
                }

            }

            _loadingState = LoadingState.NotLoading;

        }

        private void GlobalVars_FileDownloadProgressUpdated(object sender, EventArgs e)
        {
            //For now do nothing. Maybe later show progress bar or something.
            //throw new NotImplementedException();
        }

    }
}
