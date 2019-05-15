using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ConventionMobile.Business;
using ConventionMobile.Data;
using ConventionMobile.Model;
using Xamarin.Forms;

namespace ConventionMobile.Views
{
    public class GenEventsLoadingView : GenContentView
    {
        public ProgressBar TotalDownloadProgressBar;
        public Label TotalDownloadCountLabel;
        public double TotalEventsDownloading = 0;

        private GenEventManager _genEventManager;
        private GenEventsLoadingViewModel _model;
        private event EventHandler OnDoneLoadingHandler;

        public GenEventsLoadingView() : base("Now Loading...")
        {
            GlobalVars.View_GenEventsLoadingView = null;

            StartLoad();

            GlobalVars.View_GenEventsLoadingView = this;
        }

        public void StartLoad()
        {
            _genEventManager = new GenEventManager(new RestService());
            _model = new GenEventsLoadingViewModel();

            TotalDownloadCountLabel = new Label
            {
                HorizontalTextAlignment = TextAlignment.Center,
                Text = "    "
            };

            TotalDownloadProgressBar = new ProgressBar
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Progress = 0
            };

            this.Content = new StackLayout
            {
                VerticalOptions = LayoutOptions.Center,
                Padding = 20,
                Children =
                {
                    //downloadCountInfoLabel,
                    TotalDownloadProgressBar,
                    TotalDownloadCountLabel
                }
            };

            Task.Factory.StartNew(GetAllGenConEvents);

            this.OnDoneLoadingHandler += (sender, args) =>
            {
                Device.BeginInvokeOnMainThread(() => { this.IsVisible = false; });
            };
        }

        private async Task GetAllGenConEvents()
        {
            _genEventManager.TotalEventsCalculated += GenEventManager_TotalEventsCalculated;
            _genEventManager.EventsDownloadProgressCalculated += GenEventManager_EventsDownloadProgressCalculated;

            var allEvents = new List<GenEvent>();

            try
            {
                var lastSyncTime = await GlobalVars.serverLastSyncTime();
                allEvents = await _genEventManager.GetEventsAsync(lastSyncTime);
            }
            catch (Exception)
            {

            }

            if (allEvents.Count > 0)
            {
                GlobalVars.db.SaveItems(allEvents);
                GlobalVars.hasSuccessfullyLoadedEvents = true;
                GlobalVars.lastSyncTime = DateTime.Now;
            }
            else
            {
                //should probably show an error message here, bro
            }

            _genEventManager.TotalEventsCalculated -= GenEventManager_TotalEventsCalculated;
            _genEventManager.EventsDownloadProgressCalculated -= GenEventManager_EventsDownloadProgressCalculated;

            if (GlobalVars.hasSuccessfullyLoadedEvents)
            {
                await _model.CheckForNewGlobalVarsAsync(false);
                OnDoneLoadingHandler?.Invoke(null, EventArgs.Empty);
            }
        }

        private void GenEventManager_EventsDownloadProgressCalculated(object sender, EventArgs e)
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                if (TotalEventsDownloading > ((DownloadUpdateEventArgs)e).number)
                {
                    this.IsVisible = true;
                }
                else if (TotalEventsDownloading <= 0)
                {
                    this.IsVisible = false;
                }
                
                TotalDownloadProgressBar.Progress = ((DownloadUpdateEventArgs)e).number / TotalEventsDownloading;
                if (((DownloadUpdateEventArgs)e).number < TotalEventsDownloading)
                {
                    TotalDownloadCountLabel.Text = string.Format("Downloaded {0}/{1} events...", ((DownloadUpdateEventArgs)e).number, TotalEventsDownloading);
                }
                else
                {
                    try
                    {
                        TotalDownloadCountLabel.Text = "Downloaded events! Now sorting and committing to database...";
                    }
                    catch (Exception)
                    {

                    }
                }
            });
        }

        private void GenEventManager_TotalEventsCalculated(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                TotalEventsDownloading = ((DownloadUpdateEventArgs)e).number;
            });
        }
    }
}
