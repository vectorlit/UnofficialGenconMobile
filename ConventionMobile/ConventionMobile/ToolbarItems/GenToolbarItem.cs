using System;
using Xamarin.Forms;

namespace ConventionMobile.ToolbarItems
{
    public abstract class GenToolbarItem : ContentView
    {
        public event EventHandler OnClickHandler;

        protected void Initialize()
        {
            this.image = new Image
            {
                Source = ImageSource
            };

            this.label = new Label
            {
                Text = Title,
                FontSize = 8,
                HorizontalTextAlignment = TextAlignment.Center
            };

            this.layout = new StackLayout
            {
                Children =
                {
                    image,
                    label
                },
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Orientation = StackOrientation.Vertical,
            };

            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += OnClickHandler;
            layout.GestureRecognizers.Add(tapGesture);

            this.Content = layout;
        }

        public void OnClick(object sender, EventArgs e)
        {
            OnClickHandler?.Invoke(sender, e);
        }

        private StackLayout layout;
        private Image image;
        private Label label;

        public string ImageSource = "";
        public string Title = "";
    }
}
