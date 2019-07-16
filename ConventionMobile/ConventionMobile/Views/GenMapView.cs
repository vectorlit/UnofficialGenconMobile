using System;
using System.Net.Mime;
using ConventionMobile.Pages;
using Plugin.Share;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace ConventionMobile.Views
{
    public class GenMapView : GenContentView
    {
        private readonly GenMainPage _parentPage;
        public ListView navigationListView;

        public void ClearNavOption(BindableProperty property)
        {
            navigationListView?.ClearValue(property);
        }

        public GenMapView(GenMainPage parentPage) : base(GlobalVars.navigationTitle)
        {
            GlobalVars.View_GenMapView = null;
            this._parentPage = parentPage;

            navigationListView = new ListView
            {
                ItemTemplate = new DataTemplate(typeof(NavigationCell)),
                ItemsSource = GlobalVars.NavigationChoices
            };

            navigationListView.ItemTapped += (object sender, ItemTappedEventArgs e) =>
            {
                //GlobalVars.GenConBusiness.ShowLoadingEventMessage("Data is still loading, map may not be up to date");

                if (e != null && e.Item != null)
                {
                    var selectedDetailChoice = (DetailChoice)e.Item;

                    if (selectedDetailChoice.data.ToLower().StartsWith("http:") || selectedDetailChoice.data.ToLower().StartsWith("https:"))
                    {
                        // don't put this in a popup container - it uses a plugin to determine the fastest option to open (uses embedded chrome options)
                        CrossShare.Current.OpenBrowser(selectedDetailChoice.data, null);
                    }
                    else
                    {
                        var page = (PopupPage)Activator.CreateInstance(selectedDetailChoice.pageType);
                        page.BindingContext = selectedDetailChoice;
                        PopupNavigation.Instance.PushAsync(page);
                    }

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        navigationListView.SelectedItem = null;
                    });
                }
            };

            this.OnAppearedHandler += (sender, args) =>
            {
                ClearNavOption(ListView.SelectedItemProperty);
            };

            var content = new StackLayout
            {
                Children =
                {
                    navigationListView
                }
            };

            this.ToolbarItems.Add(new ToolbarItem("Font Size", "baseline_format_size_black_24.png", () =>
            {
                var page = new DisplayOptionsPage();
                PopupNavigation.Instance.PushAsync(page);
            }));

            this.ToolbarItems.Add(new ToolbarItem("Refresh", "ic_refresh_black_24dp.png", () =>
            {
                GlobalVars.View_GenEventsLoadingView.StartLoad();
            }));

            this.Content = content;
            GlobalVars.View_GenMapView = this;
        }
    }
}

