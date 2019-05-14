using Rg.Plugins.Popup.Services;

namespace ConventionMobile.ToolbarItems
{
    public class CloseEventPageToolbarItem : GenToolbarItem
    {
        private const string ImageSource = "ic_cancel_black_24dp.png";

        public CloseEventPageToolbarItem()
        {
            this.Source = ImageSource;
            this.OnClickHandler += async (sender, args) => { await PopupNavigation.Instance.PopAsync(); };
            this.AddGesture();
        }
    }
}
