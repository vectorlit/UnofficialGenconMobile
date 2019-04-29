using ConventionMobile.Data;
using ConventionMobile.Model;
using ConventionMobile.Views;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation (XamlCompilationOptions.Compile)]
namespace ConventionMobile
{
	public partial class App : Application
	{
        //public GlobalVars globalVars = new GlobalVars();
        
        public static GenEventManager GenEventManager { get; private set; }

        public Label totalDownloadCountLabel;
        public ProgressBar totalDownloadProgressBar;
        public double totalEventsDownloading = 0;

        //public HomePage homePage;
        public GenHomeTabPage homePage;

        public bool requestQuit = false;

        public App ()
		{
			InitializeComponent();

            //ShowLoadingPage();

            GlobalVars.db = GenconMobileDatabase.Create();
            Initialize();

            //showMainPage();
            //Task.Run(this.threadObserver);
        }

        //private void ShowLoadingPage()
        //{
        //    Task.Factory.StartNew(async () =>
        //    {
        //        Device.BeginInvokeOnMainThread(() =>
        //        {
        //            AbsoluteLayout absoluteLayout = new AbsoluteLayout
        //            {
        //                BackgroundColor = Color.White,
        //                VerticalOptions = LayoutOptions.FillAndExpand,
        //                HorizontalOptions = LayoutOptions.FillAndExpand
        //            };

        //            Image iconImage = new Image
        //            {
        //                Source = ImageSource.FromResource("ConventionMobile.Resources.icon.png")
        //            };

        //            absoluteLayout.Children.Add(iconImage);

        //            AbsoluteLayout.SetLayoutBounds(absoluteLayout, new Rectangle(0, 0, 1, 1));
        //            AbsoluteLayout.SetLayoutFlags(absoluteLayout, AbsoluteLayoutFlags.All);

        //            AbsoluteLayout.SetLayoutFlags(iconImage, AbsoluteLayoutFlags.PositionProportional);
        //            AbsoluteLayout.SetLayoutBounds(iconImage,
        //             new Rectangle(0.5,
        //                           0.5, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));

        //            MainPage = new ContentPage
        //            {
        //                Content = absoluteLayout
        //            };
        //        });
        //    });
        //}

        private void Initialize()
        {
            GenEventManager = new GenEventManager(new RestService());

            if (GlobalVars.hasSuccessfullyLoadedEvents && GlobalVars.eventCount > 0)
            {
                ShowMainPage();
            }
            else
            {
                ShowLoader();

                Task.Factory.StartNew(GetAllGenconEvents);
            }
        }

        private void Startup()
        {
            MainPage = new ContentPage
            {
                Content = new StackLayout
                {
                    VerticalOptions = LayoutOptions.Center,
                    Children = {
                        new Label {
                            HorizontalTextAlignment = TextAlignment.Center,
                            Text = GlobalVars.appTitle + " Loading..."
                        }
                    }
                }
            };
        }

        private void ShowLoader()
        {
            totalDownloadCountLabel = new Label
            {
                HorizontalTextAlignment = TextAlignment.Center,
                Text = "    "
            };

            Label downloadCountInfoLabel = new Label
            {
                HorizontalTextAlignment = TextAlignment.Center,
                Text = "Now downloading all " + GlobalVars.shortTitle + " events...\r\nThis is the whole catalog, so please be patient!"
            };

            totalDownloadProgressBar = new ProgressBar
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Progress = 0
            };

            MainPage = new ContentPage
            {
                Content = new StackLayout
                {
                    VerticalOptions = LayoutOptions.Center,
                    Padding = 20,
                    Children =
                        {
                            downloadCountInfoLabel,
                            totalDownloadProgressBar,
                            totalDownloadCountLabel
                        }
                }
            };
        }
        
        public void ShowMainPage()
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                //homePage = new HomePage();
                homePage = new GenHomeTabPage();
                MainPage = new NavigationPage(homePage);
                //MainPage = homePage;
            });
        }

        private async Task GetAllGenconEvents()
        {
            GenEventManager.TotalEventsCalculated += GenEventManager_TotalEventsCalculated;
            GenEventManager.EventsDownloadProgressCalculated += GenEventManager_EventsDownloadProgressCalculated;

            List<GenEvent> allEvents = new List<GenEvent>();

            try
            {
                allEvents = await GenEventManager.GetAllEventsAsync();
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

            GenEventManager.TotalEventsCalculated -= GenEventManager_TotalEventsCalculated;
            GenEventManager.EventsDownloadProgressCalculated -= GenEventManager_EventsDownloadProgressCalculated;

            if (GlobalVars.hasSuccessfullyLoadedEvents)
            {
                ShowMainPage();
            }
            else
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Startup();
                });
            }
        }

        //private async Task GetAllGenconEventsAsync()
        //{
        //    GenEventManager.TotalEventsCalculated += GenEventManager_TotalEventsCalculated;
        //    GenEventManager.EventsDownloadProgressCalculated += GenEventManager_EventsDownloadProgressCalculated;

        //    var allEvents = await GenEventManager.GetAllEventsAsync();
        //    if (allEvents.Count > 0)
        //    {
        //        await GlobalVars.db.SaveItemsAsync(allEvents);
        //        GlobalVars.hasSuccessfullyLoadedEvents = true;
        //        GlobalVars.lastSyncTime = DateTime.Now;
        //    }
        //    else
        //    {
        //        //should probably show an error message here, bro
        //    }

        //    GenEventManager.TotalEventsCalculated -= GenEventManager_TotalEventsCalculated;
        //    GenEventManager.EventsDownloadProgressCalculated -= GenEventManager_EventsDownloadProgressCalculated;

        //    if (GlobalVars.hasSuccessfullyLoadedEvents)
        //    {
        //        requestQuit = true;
        //    }
        //    else
        //    {
        //        Startup();
        //    }
        //}

        private void GenEventManager_EventsDownloadProgressCalculated(object sender, EventArgs e)
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                totalDownloadProgressBar.Progress = ((DownloadUpdateEventArgs)e).number / totalEventsDownloading;
                if (((DownloadUpdateEventArgs)e).number < totalEventsDownloading)
                {
                    totalDownloadCountLabel.Text = String.Format("Downloaded {0}/{1} events...", ((DownloadUpdateEventArgs)e).number, totalEventsDownloading);
                }
                else
                {
                    try
                    {
                        totalDownloadCountLabel.Text = String.Format("Downloaded events! Now sorting and committing to database...");
                    }
                    catch (Exception)
                    {

                    }
                }
            });
        }

        private void GenEventManager_TotalEventsCalculated(object sender, EventArgs e)
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                totalEventsDownloading = ((DownloadUpdateEventArgs)e).number;
            });
        }

        protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
