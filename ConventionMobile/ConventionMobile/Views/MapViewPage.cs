using System;
using System.IO;
using System.Reflection;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace ConventionMobile.Views
{
    public class MapViewPage : PopupPage
    {
        private readonly CustomWebView _customWeb;

        public MapViewPage()
        {
            Padding = new Thickness(0);
            _customWeb = new CustomWebView
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            var closeButton = new Button
            {
                Text = "Close",
                BackgroundColor = Color.Red,
                TextColor = Color.White
            };

            closeButton.Clicked += async (sender, args) => { await PopupNavigation.Instance.PopAsync(); };
            Content = new StackLayout
            {
                Children =
                {
                    _customWeb,
                    closeButton
                }
            };
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if (this.BindingContext is DetailChoice dc)
            {
                var fileName = DependencyService.Get<IFileOps>().GetFileLocation(dc.data);
                _customWeb.Uri = DependencyService.Get<IFileOps>().FileExists(fileName) ? fileName : dc.data;
            }
        }
    }
}
