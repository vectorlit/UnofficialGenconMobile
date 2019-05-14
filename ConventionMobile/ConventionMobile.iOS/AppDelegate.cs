using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using UIKit;
using UserNotifications;
using Xamarin.Forms;

namespace ConventionMobile.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Rg.Plugins.Popup.Popup.Init();
            global::Xamarin.Forms.Forms.Init();

            DependencyService.Register<SQLite_iOS>();
            DependencyService.Register<Calendar_iOS>();
            DependencyService.Register<FileOps_iOS>();
            DependencyService.Register<SafeAreaInsets_iOS>();
            //DependencyService.Register<ToastNotification>(); // Register your dependency
            //ToastNotification.Init();

            LoadApplication(new App());

            //if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            //{
            //    // Request Permissions
            //    UNUserNotificationCenter.Current.RequestAuthorization(UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound, (granted, error) =>
            //    {
            //        // Do something if needed
            //    });
            //}
            //else if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            //{
            //    var notificationSettings = UIUserNotificationSettings.GetSettingsForTypes(
            //     UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound, null
            //        );

            //    app.RegisterUserNotificationSettings(notificationSettings);
            //}

            return base.FinishedLaunching(app, options);
        }

        public override bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
        {
            bool success = false;
            try
            {
                if (url != null)
                {
                    NSUrlComponents urlComponents = new NSUrlComponents(url, false);

                    string data = "";
                    string name = "";

                    NSUrlQueryItem[] allItems = urlComponents.QueryItems;
                    foreach (NSUrlQueryItem item in allItems)
                    {
                        if (item.Name == "data")
                        {
                            data = item.Value;
                        }
                        else if (item.Name == "name")
                        {
                            name = item.Value;
                        }
                    }

                    if (!string.IsNullOrEmpty(data) && !string.IsNullOrEmpty(name))
                    {
                        success = true;
                        Task.Factory.StartNew(async () =>
                        {
                            await GlobalVars.ImportUserEventList(data, name);
                        });
                    }
                }
            }
            catch (Exception)
            {

            }
            
            //if (success)
            //{
            //    return base.OpenUrl(application, url, sourceApplication, annotation);
            //}

            return success;
            // 
        }
    }
}
