﻿using System;
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
        public GenSearchView genSearchView;
        public GenUserListView genUserListView;
        public GenMapView genMapView;

        public GenMainTabbedView(GenMainPage parentPage)
        {
            _parentPage = parentPage;
            genSearchView = new GenSearchView(_parentPage);
            genMapView = new GenMapView(_parentPage);
            genUserListView = new GenUserListView(_parentPage);

            _tabList = new List<GenContentView> //needed for tab controls
            {
                genMapView,
                genSearchView,
                genUserListView
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

        private void TabView_PositionChanged(object sender, Xam.Plugin.TabView.PositionChangedEventArgs e)
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
