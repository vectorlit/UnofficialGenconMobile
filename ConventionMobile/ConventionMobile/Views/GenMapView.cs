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
        private readonly ListView _nagivationListView;

        public void ClearNavOption(BindableProperty property)
        {
            _nagivationListView?.ClearValue(property);
        }

        public GenMapView(GenMainPage parentPage) : base(GlobalVars.navigationTitle)
        {
            GlobalVars.View_GenMapView = null;
            this._parentPage = parentPage;

            _nagivationListView = new ListView
            {
                ItemTemplate = new DataTemplate(typeof(NavigationCell)),
                ItemsSource = GlobalVars.NavigationChoices
            };

            _nagivationListView.ItemSelected += (async (sender, args) => {

                GlobalVars.GenConBusiness.ShowLoadingEventMessage("Data is still loading, map may not be up to date");

                if (args.SelectedItem != null)
                {
                    var selectedDetailChoice = (DetailChoice)args.SelectedItem;

                    if (selectedDetailChoice.data.ToLower().StartsWith("http:") || selectedDetailChoice.data.ToLower().StartsWith("https:"))
                    {
                        //todo put this in a popup container as well???
                        await CrossShare.Current.OpenBrowser(selectedDetailChoice.data, null);
                    }
                    else
                    {
                        var page = (PopupPage)Activator.CreateInstance(selectedDetailChoice.pageType);
                        page.BindingContext = selectedDetailChoice;
                        await PopupNavigation.Instance.PushAsync(page);
                    }
                }
            });

            this.OnAppearedHandler += (sender, args) =>
            {
                ClearNavOption(ListView.SelectedItemProperty);
            };

            var content = new StackLayout
            {
                Children =
                {
                    _nagivationListView
                }
            };

            this.Content = content;
            GlobalVars.View_GenMapView = this;
        }
    }
}

