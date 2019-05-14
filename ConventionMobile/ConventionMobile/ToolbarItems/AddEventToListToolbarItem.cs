using ConventionMobile.Model;
using ConventionMobile.Views;

namespace ConventionMobile.ToolbarItems
{
    public class AddEventToListToolbarItem : GenToolbarItem
    {
        private const string ImageSource = "addlist.png";
        private readonly GenEventFull _eventPage;

        public AddEventToListToolbarItem(GenEventFull eventPage)
        {
            this._eventPage = eventPage;
            this.Source = ImageSource;
            this.OnClickHandler += (sender, args) => { OpenAddToListPrompt(); };
            this.AddGesture();
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
