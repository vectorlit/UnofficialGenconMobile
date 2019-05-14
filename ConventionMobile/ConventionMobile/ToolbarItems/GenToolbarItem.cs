using System;
using Xamarin.Forms;

namespace ConventionMobile.ToolbarItems
{
    public abstract class GenToolbarItem : Image
    {
        public event EventHandler OnClickHandler;

        // todo: find a better way
        // kinda hacky, might want find a better way of doing this.
        // the gestures need to be loaded after the parent class
        // has been loaded.  Cannot do it in the GenToolbarItem ctor
        protected void AddGesture()
        {
            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += OnClickHandler;
            this.GestureRecognizers.Add(tapGesture);
        }

        public void OnClick(object sender, EventArgs e)
        {
            OnClickHandler?.Invoke(sender, e);
        }
    }
}
