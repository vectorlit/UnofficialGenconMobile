using ConventionMobile.Views;
using ModernHttpClient;
using Plugin.Share;
//using Plugin.Toasts;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ConventionMobile.Views
{
    public class GenHomeTabPage : TabbedPage
    {
        //GenSearchPage searchPage;
        GenSearchPage searchPage = null;
        public UserListPage userListPage;
        ListView navigationListView;
        public bool overrideUpdateCheckEvents = false;
        public bool overrideUpdateCheckOptions = false;

        protected override void OnCurrentPageChanged()
        {
            CheckForUserListPageListRefresh();
            base.OnCurrentPageChanged();
        }

        public void CheckForUserListPageListRefresh()
        {
            if (this.CurrentPage.Title == GlobalVars.userListsTitle)
            {
                try
                {
                    if (userListPage.IsUpdateRequested || ((App)Application.Current).homePage.userListPage.IsUpdateRequested)
                    {
                        userListPage.IsUpdateRequested = false;
                        ((App)Application.Current).homePage.userListPage.IsUpdateRequested = false;
                        userListPage.UpdateUserLists();
                    }
                    //UserListPage updatePage = this.CurrentPage as UserListPage;
                    //updatePage.UpdateUserLists();
                }
                catch (Exception)
                {

                }
            }
        }
       

        public GenHomeTabPage()
        {

            navigationListView = new ListView
            {
                ItemTemplate = new DataTemplate(typeof(NavigationCell)),
                ItemsSource = GlobalVars.NavigationChoices
            };

            this.Title = GlobalVars.appTitle;

            Command<Type> navigateCommand =
               new Command<Type>(async (Type pageType) =>
               {
                   Page page = (Page)Activator.CreateInstance(pageType);
                   await this.Navigation.PushAsync(page);
               });

            var MapPage = new ContentPage
            {
                Title = GlobalVars.navigationTitle,
                Content = new StackLayout
                {
                    // Edit children here to add additional navigation options besides just maps.
                    Children =
                    {
                        //headerLayout,
                        navigationListView
                    }
                }
            };

            searchPage = new GenSearchPage();

            userListPage = new UserListPage();
            
            Children.Add(MapPage);
            Children.Add(searchPage);
            Children.Add(userListPage);

            Xamarin.Forms.PlatformConfiguration.AndroidSpecific.TabbedPage.SetIsSwipePagingEnabled(this, true);

            // Define a selected handler for the ListView.
            navigationListView.ItemSelected += (async (sender, args) => {

                if (args.SelectedItem != null)
                {
                    DetailChoice selectedDetailChoice = (DetailChoice)args.SelectedItem;

                    if (selectedDetailChoice.data.ToLower().StartsWith("http:") || selectedDetailChoice.data.ToLower().StartsWith("https:"))
                    {
                        await CrossShare.Current.OpenBrowser(selectedDetailChoice.data, null);
                    }
                    else
                    {
                        Page page = (Page)Activator.CreateInstance(selectedDetailChoice.pageType);
                        page.BindingContext = selectedDetailChoice;
                        //this.IsPresented = false;
                        await this.Navigation.PushAsync(page);
                    }
                }
            });
        }


        public async Task CheckForNewEventsAsync()
        {
            bool dontShowFurtherToasts = false;
            if (GlobalVars.isSyncNeeded || overrideUpdateCheckEvents)
            {
                overrideUpdateCheckEvents = false;
                //bool tapped = await GlobalVars.notifier.Notify(ToastNotificationType.Info,
                //"Updating", "Now checking for updated events...", TimeSpan.FromSeconds(2));
                //var tapped = await GlobalVars.notifier.Notify(new NotificationOptions()
                //{
                //    Title = "Updating",
                //    Description = "Now checking for updated events...",
                //});
                GlobalVars.DoToast("Now checking for updated events...", GlobalVars.ToastType.Yellow);
                var events = await App.GenEventManager.GetEventsAsync(await GlobalVars.serverLastSyncTime());

                string toastText = "";

                if (events.Count > 0)
                {
                    toastText = await GlobalVars.db.SaveItemsAsync(events);
                }

                GlobalVars.lastSyncTime = DateTime.Now;

                //bool tapped2 = await GlobalVars.notifier.Notify(ToastNotificationType.Success,
                //"Events Updated", "All events are now up-to-date.", TimeSpan.FromSeconds(2));
                //var tapped2 = await GlobalVars.notifier.Notify(new NotificationOptions()
                //{
                //    Title = "Events Updated.",
                //    Description = "All events are now up-to-date."
                //});

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
            }

            await CheckForNewGlobalVarsAsync(dontShowFurtherToasts);
        }

        private async Task CheckForNewGlobalVarsAsync(bool dontShowFurtherToasts)
        {
            //string newURL = String.Format(GlobalVars.GlobalOptionsURLCustomizableURL, TimeZoneInfo.ConvertTime(GlobalVars.lastGlobalVarUpdateTime, TimeZoneInfo.Utc).ToString("yyyy-MM-dd't'HH:mm:ss"));

            if (GlobalVars.isGlobalVarSyncNeeded || overrideUpdateCheckOptions)
            {
                overrideUpdateCheckOptions = false;
                //bool tapped = await GlobalVars.notifier.Notify(ToastNotificationType.Info,
                //"Updating Maps and other info", "Now checking for updated information...", TimeSpan.FromSeconds(2));
                //var returned = await GlobalVars.notifier.Notify(new NotificationOptions()
                //{
                //    Title = "Updating Maps and other info",
                //    Description = "Now checking for updated information..."
                //});
                if (!dontShowFurtherToasts)
                {
                    GlobalVars.DoToast("Now checking for updated maps or other info...", GlobalVars.ToastType.Yellow);
                }

                //using (var client = new HttpClient(new NativeMessageHandler()
                //{
                //    Timeout = new TimeSpan(0, 0, 9),
                //    EnableUntrustedCertificates = true,
                //    DisableCaching = true
                //}))
                using (var client = new HttpClient())
                {
                    client.MaxResponseContentBufferSize = 25600000;
                    DateTime indyTime = DependencyService.Get<ICalendar>().ConvertToIndy(GlobalVars.lastGlobalVarUpdateTime);
                    string newURL = String.Format(GlobalVars.GlobalOptionsURLCustomizableURL, indyTime.ToString("yyyy-MM-dd't'HH:mm:ss"));

                    try
                    {
                        var response = await client.GetAsync(newURL).ConfigureAwait(false);

                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                            if (content.Length > 10)
                            {
                                GlobalVars.FileDownloadProgressUpdated += GlobalVars_FileDownloadProgressUpdated;

                                bool totalSuccess = await GlobalVars.OverwriteOptions(content);

                                GlobalVars.FileDownloadProgressUpdated -= GlobalVars_FileDownloadProgressUpdated;

                                //START HERE DUMMY
                                // PICK UP CODING HERE STUPID


                                //bool tapped2 = await GlobalVars.notifier.Notify(ToastNotificationType.Success,
                                //"Maps and info have been updated.", "**REFRESHING SCREEN**", TimeSpan.FromSeconds(2));
                                //var tapped2 = await GlobalVars.notifier.Notify(new NotificationOptions()
                                //{
                                //    Title = "Maps and info have been updated.",
                                //    Description = "**REFRESHING SCREEN**",
                                //});
                                GlobalVars.DoToast("Update success - **REFRESHING SCREEN**", GlobalVars.ToastType.Green);
                                searchPage?.UpdateEventInfo();

                                await searchPage?.CloseAllPickers();
                                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                                {
                                    try
                                    {
                                        App.Current.MainPage = new NavigationPage(new GenHomeTabPage());
                                    }
                                    catch (Exception)
                                    {

                                    }
                                });


                            }
                            else
                            {
                                //bool tapped2 = await GlobalVars.notifier.Notify(ToastNotificationType.Success,
                                //"Check complete.", "You already have the most recent info.", TimeSpan.FromSeconds(2));
                                //var tapped2 = await GlobalVars.notifier.Notify(new NotificationOptions()
                                //{
                                //    Title = "Check complete.",
                                //    Description = "You already have the most recent info."
                                //});
                                if (!dontShowFurtherToasts)
                                {
                                    GlobalVars.DoToast("You are up to date.", GlobalVars.ToastType.Green);
                                }
                                
                                try
                                {
                                    Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
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
                        //bool tapped2 = await GlobalVars.notifier.Notify(ToastNotificationType.Success,
                        //        "Check complete.", "Couldn't update now, but we'll try again later.", TimeSpan.FromSeconds(2));
                        //var tapped2 = await GlobalVars.notifier.Notify(new NotificationOptions()
                        //{
                        //    Title = "Check complete.",
                        //    Description = "Couldn't update now, but we'll try again later."
                        //});
                        if (!dontShowFurtherToasts)
                        {
                            GlobalVars.DoToast("Couldn't update now, but we'll try again later.", GlobalVars.ToastType.Yellow);
                        }
                    }
                }

            }
        }

        private void GlobalVars_FileDownloadProgressUpdated(object sender, EventArgs e)
        {
            //For now do nothing. Maybe later show progress bar or something.
            //throw new NotImplementedException();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Task.Factory.StartNew(CheckForNewEventsAsync);
            if (navigationListView != null)
            {
                navigationListView.ClearValue(ListView.SelectedItemProperty);
            }
            CheckForUserListPageListRefresh();
        }


    }
}
