using System;
using Xamarin.Forms;

namespace ConventionMobile.Views
{
    public class ClosableNotificationBox : ContentView
    {
        private bool _allowTouchClose = true;

        public ClosableNotificationBox()
        {
            this.IsVisible = false;
            this.HeightRequest = 150; // might want to make this adjustable based on the device's screen size

            var closeTapGesture = new TapGestureRecognizer();
            closeTapGesture.Tapped += _closeTapGesture_Tapped;
            this.GestureRecognizers.Add(closeTapGesture);
        }

        public void UpdateView(View view, bool allowTouchClose = true)
        {
            if (this.IsVisible)
            {
                this.CloseView();
            }

            _allowTouchClose = allowTouchClose;
            this.IsVisible = true;
            this.Content = view;
        }

        public void CloseView()
        {
            //maybe wanna animate away

            this.IsVisible = false;
            this.Content = null;
        }

        private void _closeTapGesture_Tapped(object sender, EventArgs e)
        {
            if (_allowTouchClose)
            {
                this.CloseView();
            }
        }
    }
}
