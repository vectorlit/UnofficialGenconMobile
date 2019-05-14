using ConventionMobile.Model;
using ConventionMobile.Views;
using Plugin.Share;

namespace ConventionMobile.ToolbarItems
{
    public class ShareEventToolbarItem : GenToolbarItem
    {
        private const string ImageSource = "ic_share_black_24dp.png";

        public ShareEventToolbarItem(GenEventFull eventPage)
        {
            this.Source = ImageSource;

            this.OnClickHandler += (sender, args) =>
            {
                var currentEvent = (GenEvent)eventPage.BindingContext;
                CrossShare.Current.Share(new Plugin.Share.Abstractions.ShareMessage
                {
                    Url = currentEvent.LiveURL,
                    Text = currentEvent.Description,
                    Title = currentEvent.Title
                },
                new Plugin.Share.Abstractions.ShareOptions
                {
                    ChooserTitle = "Share Event"
                });
            };

            this.AddGesture();
        }
    }
}
