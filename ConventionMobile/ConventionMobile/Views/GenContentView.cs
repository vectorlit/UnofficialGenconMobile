using System;
using System.Collections.Generic;
using Xam.Plugin.TabView;
using Xamarin.Forms;

namespace ConventionMobile.Views
{
    public abstract class GenContentView : ContentView
    {
        public readonly string Title;
        public readonly List<ToolbarItem> ToolbarItems = new List<ToolbarItem>();

        public event EventHandler OnAppearedHandler;
        public event EventHandler OnAppearingHandler;

        public event EventHandler OnLeavingHandler;
        public event EventHandler OnLeftHandler;

        public event EventHandler CheckForUpdateHandler;
        
        protected GenContentView(string title)
        {
            Title = title;
        }

        public TabItem AsTabItem()
        {
            return new TabItem(this.Title, this.Content);
        }

        public void OnAppeared(object sender, EventArgs args)
        {
            OnAppearedHandler?.Invoke(sender, args);
        }

        public void OnAppearing(object sender, EventArgs args)
        {
            OnAppearingHandler?.Invoke(sender, args);
        }

        public void OnLeaving(object sender, EventArgs args)
        {
            OnLeavingHandler?.Invoke(sender, args);
        }

        public void OnLeft(object sender, EventArgs args)
        {
            OnLeftHandler?.Invoke(sender, args);
        }

        public void CheckForUpdate(object sender, EventArgs args)
        {
            CheckForUpdateHandler?.Invoke(sender, args);
        }
    }
}
