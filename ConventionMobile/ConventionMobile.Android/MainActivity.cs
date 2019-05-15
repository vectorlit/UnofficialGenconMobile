using System;

using Android.App;
using Android.Content.PM;
using Android.OS;
using Xamarin.Forms;
using Acr.UserDialogs;
using Plugin.CurrentActivity;
using Android.Content;
using System.Threading.Tasks;
//using Plugin.Toasts;

namespace ConventionMobile.Droid
{
    [Activity(Label = "Unofficial Gen Con 2019", Icon = "@mipmap/cm_launcher", RoundIcon = "@mipmap/cm_launcher_round", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    [IntentFilter(new [] { Intent.ActionView },
        Categories = new[] {
            Intent.ActionView,
            Intent.CategoryDefault,
            Intent.CategoryBrowsable
        },
        DataScheme = "genconevents"
        )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        Context CurrentContext => CrossCurrentActivity.Current.Activity;

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);
            Rg.Plugins.Popup.Popup.Init(this, bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);

            DependencyService.Register<Calendar_Android>();
            DependencyService.Register<FileOps_Android>();
            DependencyService.Register<SQLite_Android>();
            DependencyService.Register<SafeAreaInsets_Android>();

            UserDialogs.Init(() => (Activity)CurrentContext);
            
            if (Intent != null && Intent.DataString != null)
            {
                try
                {
                    string name = "";
                    string data = "";
                    name = Intent.Data.GetQueryParameter("name");
                    data = Intent.Data.GetQueryParameter("data");

                    if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(data))
                    {
                        Task.Factory.StartNew(async () =>
                        {
                            await GlobalVars.ImportUserEventList(data, name);
                        });
                    }
                }
                catch (Exception)
                {

                }
            }

            LoadApplication(new App());
        }

        public override void OnBackPressed()
        {
            if (Rg.Plugins.Popup.Popup.SendBackPressed(base.OnBackPressed))
            {
                // Do something if there are some pages in the `PopupStack`
            }
            else
            {
                // Do something if there are not any pages in the `PopupStack`
            }
        }
    }
}

