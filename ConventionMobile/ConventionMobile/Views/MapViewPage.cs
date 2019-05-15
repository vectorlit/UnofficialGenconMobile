using System;
using System.IO;
using System.Reflection;
using ConventionMobile.ToolbarItems;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace ConventionMobile.Views
{
    public class MapViewPage : PopupPage
    {
        private readonly CustomWebView _customWeb;
        private Label titleLabel;

        public MapViewPage()
        {
            Padding = new Thickness(0);
            _customWeb = new CustomWebView
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            BackgroundColor = Color.Transparent;
            
            titleLabel = new Label
            {
                Text = "",
                FontSize = GlobalVars.sizeLarge,
                FontAttributes = FontAttributes.Bold,
                TextColor = GlobalVars.ThemeColorsText[(int)GlobalVars.ThemeColors.Primary],
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalTextAlignment = TextAlignment.Center
            };

            var titleBar = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Children =
                {
                    new CloseEventPageToolbarItem(),
                    titleLabel
                },
                BackgroundColor = GlobalVars.ThemeColorsBG[(int)GlobalVars.ThemeColors.Primary],
            };

            Content = new StackLayout
            {
                Children =
                {
                    titleBar,
                    _customWeb
                },
                Spacing = 0
            };
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if (this.BindingContext is DetailChoice dc)
            {
                var fileName = DependencyService.Get<IFileOps>().GetFileLocation(dc.data);
                _customWeb.Uri = DependencyService.Get<IFileOps>().FileExists(fileName) ? fileName : dc.data;
                titleLabel.Text = dc.title;
            }
        }
    }
}
