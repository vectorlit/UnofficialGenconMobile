using System;
using System.Collections.Generic;
using System.Linq;
using ConventionMobile.Pages;
using Xam.Plugin.TabView;
using Xamarin.Forms;

namespace ConventionMobile.Views
{
    public class GenMainTabbedView : ContentView
    {
        private readonly GenMainPage _parentPage;
        private readonly List<GenContentView> _tabList;

        public GenMainTabbedView(GenMainPage parentPage)
        {
            _parentPage = parentPage;

            _tabList = new List<GenContentView> //needed for tab controls
            {
                new GenMapView(_parentPage),
                new GenSearchView(_parentPage),
                new GenUserListView(_parentPage)
            };

            // main tabbed view
            var tabViewControl = new TabViewControl(_tabList.Select(x => x.AsTabItem()).ToList());
            tabViewControl.HeaderBackgroundColor = GlobalVars.ThemeColorsBG[(int)GlobalVars.ThemeColors.Secondary];
            tabViewControl.HeaderTabTextColor = GlobalVars.ThemeColorsText[(int)GlobalVars.ThemeColors.Secondary];

            this.Content = tabViewControl;
            this.VerticalOptions = LayoutOptions.FillAndExpand;
            this.HorizontalOptions = LayoutOptions.FillAndExpand;

            tabViewControl.PositionChanged += TabView_PositionChanged;
            tabViewControl.PositionChanging += TabView_PositionChanging;
        }

        private void TabView_PositionChanging(object sender, PositionChangingEventArgs e)
        {
            // trigger views OnAppearing Event
            var newTabbedPage = _tabList[e.NewPosition];
            newTabbedPage.OnAppearing(this, e);

            // triggers views OnLeaving Event
            var oldTabbedpage = _tabList[e.OldPosition];
            oldTabbedpage.OnLeaving(this, e);
        }

        private void TabView_PositionChanged(object sender, PositionChangedEventArgs e)
        {
            // update toolbar items
            if (_parentPage.ToolbarItems.Any())
            {
                _parentPage.ToolbarItems.Clear();
            }

            var newTabbedPage = _tabList[e.NewPosition];
            newTabbedPage.ToolbarItems.ForEach(toolbarItem => _parentPage.ToolbarItems.Add(toolbarItem));

            // trigger views OnAppeared Event
            newTabbedPage.OnAppeared(this, e);

            // triggers views OnLeft Event
            var oldTabbedPage = _tabList[e.OldPosition];
            oldTabbedPage.OnLeft(this, e);
        }
    }
}
