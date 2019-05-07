using System;
using System.Threading.Tasks;
using ConventionMobile.Business;
using Xamarin.Forms;

namespace ConventionMobile.Views
{
    public class GenHomeTabPage : TabbedPage
    {
        public readonly GenMapPage GenMapPage;
        public readonly GenSearchPage GenSearchPage;
        public readonly UserListPage UserListPage;
        private readonly IGenConBusiness _business;

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
                    if (UserListPage.IsUpdateRequested || ((App)Application.Current).HomePage.UserListPage.IsUpdateRequested)
                    {
                        UserListPage.IsUpdateRequested = false;
                        ((App)Application.Current).HomePage.UserListPage.IsUpdateRequested = false;
                        UserListPage.UpdateUserLists();
                    }
                }
                catch (Exception)
                {

                }
            }
        }
        
        public GenHomeTabPage(IGenConBusiness business)
        {
            _business = business;

            this.Title = GlobalVars.appTitle;

            GenMapPage = new GenMapPage(_business);
            GenSearchPage = new GenSearchPage(_business);
            UserListPage = new UserListPage();
            
            Children.Add(GenMapPage);
            Children.Add(GenSearchPage);
            Children.Add(UserListPage);

            Xamarin.Forms.PlatformConfiguration.AndroidSpecific.TabbedPage.SetIsSwipePagingEnabled(this, true);
        }
        
        protected override void OnAppearing()
        {
            base.OnAppearing();
            GenMapPage.ClearNavOption(ListView.SelectedItemProperty);
            Task.Factory.StartNew(_business.CheckForNewEventsAsync);
        }
    }
}
