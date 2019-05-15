using ConventionMobile.Model;
using ConventionMobile.Views;
using Plugin.Share;

namespace ConventionMobile.ToolbarItems
{
    public class ShareEventToolbarItem : GenToolbarItem
    {
        public ShareEventToolbarItem(GenEventFull eventPage)
        {
            this.ImageSource = "ic_share_black_24dp.png";
            this.Title = "Share Event";

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

            this.Initialize();
        }
    }
}
