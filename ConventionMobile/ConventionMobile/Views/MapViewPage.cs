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

            var leftBar = new BoxView
            {
            };

            var innerHolder = new RelativeLayout
            {
                BackgroundColor = Color.White
            };

            innerHolder.Children.Add(_customWeb, Constraint.RelativeToParent((parent) => {
                return 0;
            }), Constraint.RelativeToParent((parent) => {
                return 0;
            }), Constraint.RelativeToParent((parent) => {
                return parent.Width;
            }), Constraint.RelativeToParent((parent) => {
                return parent.Height;
            }));

            innerHolder.Children.Add(leftBar, Constraint.RelativeToParent((parent) => {
                return 0;
            }), Constraint.RelativeToParent((parent) => {
                return 0;
            }), Constraint.RelativeToParent((parent) => {
                return parent.Width * .02;
            }), Constraint.RelativeToParent((parent) => {
                return parent.Height;
            }));

            var holder = new StackLayout
            {
                Children =
                {
                    titleBar,
                    innerHolder
                },
                Spacing = 0
            };
            
            var rightSwipeGesture = new SwipeGestureRecognizer { Direction = SwipeDirection.Right, Threshold = 10000 };

            rightSwipeGesture.Swiped += OnSwiped;

            leftBar.GestureRecognizers.Add(rightSwipeGesture);

            Content = holder;
        }

        private void OnSwiped(object sender, SwipedEventArgs e)
        {
            try
            {
                PopupNavigation.Instance.PopAsync();
            }
            catch (Exception)
            {

            }
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
