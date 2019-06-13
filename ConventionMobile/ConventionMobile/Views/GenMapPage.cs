using System;
using ConventionMobile.Business;
using Plugin.Share;
using Xamarin.Forms;

namespace ConventionMobile.Views
{
    public class GenMapPage : ContentPage
    {
        private readonly ListView _nagivationListView;

        public void ClearNavOption(BindableProperty property)
        {
            _nagivationListView?.ClearValue(property);
        }

        public GenMapPage()
        {

            this.Title = GlobalVars.navigationTitle;

            _nagivationListView = new ListView
            {
                ItemTemplate = new DataTemplate(typeof(NavigationCell)),
                ItemsSource = GlobalVars.NavigationChoices
            };

            _nagivationListView.ItemSelected += (async (sender, args) => {

                GlobalVars.GenConBusiness.ShowLoadingEventMessage("Data is still loading, map may not be up to date");

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
            
            var content  = new StackLayout
            {
                Children =
                {
                    _nagivationListView
                }
            };

            this.Content = content;
        }
    }
}
