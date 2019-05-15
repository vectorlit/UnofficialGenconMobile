using ConventionMobile.Data;
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

	    public GenMainPage HomePage;

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
                HomePage = new GenMainPage();
                MainPage = new NavigationPage(HomePage);
                ((NavigationPage)MainPage).BarTextColor = GlobalVars.ThemeColorsText[(int)GlobalVars.ThemeColors.Primary];
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
