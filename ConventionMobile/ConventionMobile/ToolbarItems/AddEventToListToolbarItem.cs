using ConventionMobile.Model;
using ConventionMobile.Views;

namespace ConventionMobile.ToolbarItems
{
    public class AddEventToListToolbarItem : GenToolbarItem
    {
        private readonly GenEventFull _eventPage;

        public AddEventToListToolbarItem(GenEventFull eventPage)
        {
            this._eventPage = eventPage;
            this.ImageSource = "addlist.png";
            this.Title = "Add To List";
            this.OnClickHandler += (sender, args) => { OpenAddToListPrompt(); };
            this.Initialize();
        }

        private void OpenAddToListPrompt()
        {
            var currentEvent = (GenEvent)_eventPage.BindingContext;

            _eventPage.userEventLists = GlobalVars.db.UserEventLists;
            _eventPage.userListPicker.ItemsSource = _eventPage.UserListsTitles;
            _eventPage.popupHolder.IsVisible = true;
        }
    }
}
