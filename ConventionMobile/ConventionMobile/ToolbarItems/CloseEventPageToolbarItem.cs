using Rg.Plugins.Popup.Services;
using System;
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
                    try
                    {
                        await PopupNavigation.Instance.PopAsync();
                    }
                    catch (Exception)
                    {

                    }
                })
            });
            this.Margin = new Thickness(10);
        }
    }
}
