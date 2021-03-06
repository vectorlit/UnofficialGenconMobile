﻿using ConventionMobile.Data;
using ConventionMobile.Helpers;
using ConventionMobile.Model;
using ConventionMobile.ToolbarItems;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ConventionMobile.Views
{
    public class EventChangeLogFull : PopupPage
    {
        public ScrollView wholePageScroller;
        public StackLayout wholePageHolder;
        public StackLayout wholePage;
        public List<EventChangeLog> eventChangeLogList;

        public EventChangeLogFull()
        {
            wholePageScroller = new ScrollView { HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand, Margin = 0, Padding = 0 };

            wholePageHolder = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Padding = 0,
                Spacing = 0,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            wholePage = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Padding = 0,
                Spacing = 0,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            wholePageScroller.Content = wholePage;

            wholePageHolder.Children.Add(wholePageScroller);

            wholePageHolder.BackgroundColor = Color.White;

            Label titleLabel = new Label
            {
                FontSize = GlobalVars.sizeLarge,
                FontAttributes = FontAttributes.Bold,
                TextColor = GlobalVars.ThemeColorsText[(int)GlobalVars.ThemeColors.Primary],
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalTextAlignment = TextAlignment.Center,
                Margin = new Thickness(4)
            };
            titleLabel.SetBinding(Label.TextProperty, "Title");

            var titleBar = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Children =
                {
                    new CloseEventPageToolbarItem(),
                    new StackLayout
                    {
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        Orientation = StackOrientation.Vertical,
                        VerticalOptions = LayoutOptions.CenterAndExpand,
                        Children =
                        {
                            titleLabel
                        },
                        Margin = new Thickness(4)
                    }
                },
                BackgroundColor = GlobalVars.ThemeColorsBG[(int)GlobalVars.ThemeColors.Primary],
            };

            var actualContent = new StackLayout
            {
                Children =
                {
                    titleBar,
                    wholePageHolder
                },
                Spacing = 0
            };

            this.Content = actualContent;
        }

        protected override void OnDisappearing()
        {
            if (BindingContext is GenEvent eventItem)
            {
                Task.Factory.StartNew(async () =>
                {
                    await GlobalVars.db.DeleteAllEventChangeLogsAsync(eventChangeLogList);
                    await GlobalVars.db.SetGenEventUpdateNotificationAsReadAsync(eventItem);
                    GlobalVars.View_GenUserListView.IsUpdateRequested = true;
                });
            }
            base.OnDisappearing();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            var eventItem = BindingContext as GenEvent;
            if (eventItem != null)
            {
                Task.Factory.StartNew(async () =>
                {
                    var newWholePage = new StackLayout
                    {
                        Orientation = StackOrientation.Vertical,
                        Padding = 5,
                        Spacing = 0,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        VerticalOptions = LayoutOptions.FillAndExpand
                    };

                    eventChangeLogList = await GlobalVars.db.GetEventChangeLogsForGenEvent(eventItem);
                    if (eventChangeLogList != null && eventChangeLogList.Count > 0)
                    {
                        newWholePage.Children.Add(new Label
                        {
                            Text = "This event was updated since you last viewed it in your list!",
                            FontSize = 18,
                            Margin = new Thickness(0, 20, 0, 0),
                            HorizontalOptions = LayoutOptions.FillAndExpand
                        });

                        var changeLogs = eventChangeLogList.OrderBy(d => d.ChangeTime).ToList();

                        var changeTime = DateTime.MinValue;

                        for (var i = 0; i < changeLogs.Count; i++)
                        {
                            if (changeTime != changeLogs[i].ChangeTime)
                            {
                                changeTime = changeLogs[i].ChangeTime;

                                newWholePage.Children.Add(new Label
                                {
                                    Text = "Changes from " + changeTime.ToShortDateString() + "@" + changeTime.ToShortTimeString() + ":",
                                    FontSize = 12,
                                    FontAttributes = FontAttributes.Bold,
                                    Margin = new Thickness(5, 5, 0, 0),
                                    HorizontalOptions = LayoutOptions.FillAndExpand
                                });
                            }

                            newWholePage.Children.Add(new Label
                            {
                                Text = GenEventHelpers.GetFriendlyColumnName(changeLogs[i].Property) + " changed FROM",
                                FontSize = 12,
                                Margin = new Thickness(10, i > 0 ? 15 : 5, 0, 0),
                                HorizontalOptions = LayoutOptions.FillAndExpand
                            });

                            newWholePage.Children.Add(new Label
                            {
                                Text = changeLogs[i].OldValue,
                                FontSize = 12,
                                Margin = new Thickness(15, 0, 0, 0),
                                TextColor = Color.DarkRed,
                                HorizontalOptions = LayoutOptions.FillAndExpand
                            });

                            newWholePage.Children.Add(new Label
                            {
                                Text = "--TO--",
                                FontSize = 12,
                                Margin = new Thickness(20, 0, 0, 0),
                                HorizontalOptions = LayoutOptions.FillAndExpand
                            });

                            newWholePage.Children.Add(new Label
                            {
                                Text = changeLogs[i].NewValue,
                                FontSize = 12,
                                Margin = new Thickness(15, 0, 0, 0),
                                TextColor = Color.DarkGreen,
                                HorizontalOptions = LayoutOptions.FillAndExpand
                            });
                        }
                    }
                    else
                    {
                        newWholePage.Children.Add(new Label
                        {
                            Text = "There are no new changes detected for this event.",
                            HorizontalOptions = LayoutOptions.FillAndExpand
                        });
                    }

                    var closeButton = new Button
                    {
                        Text = "OK",
                        HorizontalOptions = LayoutOptions.FillAndExpand
                    };

                    closeButton.Clicked += CloseButton_Clicked;

                    newWholePage.Children.Add(closeButton);
                    
                    wholePageScroller.Content = newWholePage;
                });
            }
        }

        private void CloseButton_Clicked(object sender, EventArgs e)
        {
            PopupNavigation.Instance.PopAsync();
        }
    }
}
