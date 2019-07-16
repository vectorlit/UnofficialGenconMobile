using System;
using System.Collections.Generic;
using ConventionMobile.Data;
using ConventionMobile.Model;
using ConventionMobile.Pages;
using ConventionMobile.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation (XamlCompilationOptions.Compile)]
namespace ConventionMobile
{
	public partial class App : Application
	{
        public static GenEventManager GenEventManager { get; private set; }

	    public GenMainPage HomePage = null;

        public App ()
		{
			InitializeComponent();
            GlobalVars.db = GenconMobileDatabase.Create();
            Initialize();
        }

        private void Initialize()
        {
            GenEventManager = new GenEventManager(new RestService());
            ShowMainPage();
        }

        public void ShowMainPage()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                // this is an attempt to invalidate some of the list view sources before refreshing the app's view.
                // This alleviates some, but not all of the bugs associated with switching tabs quickly following a refresh
                if (HomePage != null)
                {
                    try
                    {
                        HomePage.tabbedView.genMapView.navigationListView.ItemsSource = null;
                    }
                    catch (Exception)
                    {

                    }

                    try
                    {
                        HomePage.tabbedView.genSearchView.genEventListView.ItemsSource = null;
                    }
                    catch (Exception)
                    {

                    }

                    try
                    {
                        HomePage.tabbedView.genUserListView.genEventListView.ItemsSource = null;
                    }
                    catch (Exception)
                    {

                    }
                }
                                
                HomePage = new GenMainPage();
                var mainPage = new NavigationPage(HomePage);
                mainPage.BarTextColor = GlobalVars.ThemeColorsText[(int)GlobalVars.ThemeColors.Primary];
                mainPage.BarBackgroundColor = GlobalVars.ThemeColorsBG[(int)GlobalVars.ThemeColors.Primary];
                MainPage = mainPage;
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
