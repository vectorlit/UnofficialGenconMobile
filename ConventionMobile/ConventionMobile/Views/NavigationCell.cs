using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ConventionMobile.Views
{
    class NavigationCell: ViewCell
    {
        public Image clickIcon;
        public Label viewLabel;
        
        public NavigationCell()
        {
            var wholeLayout = new Grid
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Padding = 6,
                ColumnDefinitions = new ColumnDefinitionCollection() {
                    new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Star) }
                },
                RowDefinitions = new RowDefinitionCollection() {
                    new RowDefinition() { Height = GridLength.Auto }
                }
            };

            clickIcon = new Image { MinimumWidthRequest = 20 };

            viewLabel = new Label
            {
                Text = "Menu Choice",
                LineBreakMode = LineBreakMode.WordWrap,
                FontSize = 18,
                VerticalTextAlignment = TextAlignment.Center
            };
            viewLabel.SetBinding(Label.TextProperty, "title");

            wholeLayout.Children.Add(clickIcon, 0, 0);
            wholeLayout.Children.Add(viewLabel, 1, 0);
            wholeLayout.BackgroundColor = Color.White;

            View = wholeLayout;

        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            string imageSourceString = null;

            if (this.BindingContext != null && this.BindingContext.GetType() == typeof(DetailChoice))
            {
                imageSourceString = ((DetailChoice)(this.BindingContext)).image;
            }

            if (!string.IsNullOrEmpty(imageSourceString))
            {
                if (imageSourceString.Contains(".Resources."))
                {
                    clickIcon.Source = ImageSource.FromResource(imageSourceString);
                }
                else
                {
                    clickIcon.Source = ImageSource.FromFile(imageSourceString);
                }
            }
        }


    }
}
