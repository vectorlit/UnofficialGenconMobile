using ConventionMobile.Data;
using ConventionMobile.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation (XamlCompilationOptions.Compile)]
namespace ConventionMobile
{
	public partial class App : Application
	{
        public static GenEventManager GenEventManager { get; private set; }

        public GenHomeTabPage HomePage;
        
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
                HomePage = new GenHomeTabPage();
                MainPage = new NavigationPage(HomePage);
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
