using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace ConventionMobile.ToolbarItems
{
    public class CloseEventPageToolbarItem : Image
    {
        private const string ImageSource = "baseline_arrow_back_24.png";

        public CloseEventPageToolbarItem()
        {
            this.Source = ImageSource;
            this.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () =>
                {
                    await PopupNavigation.Instance.PopAsync();
                })
            });
            this.Margin = new Thickness(10);
        }
    }
}
