using ConventionMobile.Data;
using ConventionMobile.Model;
//using Plugin.Toasts;
using Plugin.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConventionMobile.Pages;
using ConventionMobile.ToolbarItems;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;

namespace ConventionMobile.Views
{
    public class GenEventFull : PopupPage
    {
        public Grid wholePage;
        public ScrollView wholePageScroller;
        public StackLayout wholePageHolder;
        public StackLayout popupHolder;
        public ListView userListPicker;
        

        public const string newListString = "(Create a new list and add to it)";

        private Thickness paddingAmount = 0;
        public List<UserEventList> userEventLists = new List<UserEventList>();

        public List<string> UserListsTitles
        {
            get
            {
                List<string> returnMe = userEventLists.Select(d => d.Title).ToList();
                returnMe.Insert(0, newListString);
                return returnMe;
            }
        }

        public GenEventFull()
        {
            CalculatePaddingAmount();

            wholePageScroller = new ScrollView { HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill, Margin = 0, Padding = paddingAmount };

            wholePageHolder = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Padding = 0,
                Spacing = 0,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            wholePage = new Grid
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Padding = 0,
                Margin = 0,
                ColumnDefinitions = new ColumnDefinitionCollection() {
                    new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) }
                },
                RowDefinitions = new RowDefinitionCollection() {
                    new RowDefinition() { Height = GridLength.Auto },
                    new RowDefinition() { Height = GridLength.Auto },
                    new RowDefinition() { Height = GridLength.Auto },
                    new RowDefinition() { Height = GridLength.Auto },
                    new RowDefinition() { Height = GridLength.Auto },
                    new RowDefinition() { Height = GridLength.Auto },
                    new RowDefinition() { Height = GridLength.Auto },
                    new RowDefinition() { Height = GridLength.Auto },
                    new RowDefinition() { Height = GridLength.Auto },
                    new RowDefinition() { Height = GridLength.Auto },
                    new RowDefinition() { Height = GridLength.Auto },
                    new RowDefinition() { Height = GridLength.Auto },
                    new RowDefinition() { Height = GridLength.Auto },
                    new RowDefinition() { Height = GridLength.Auto },
                    new RowDefinition() { Height = GridLength.Auto },
                    new RowDefinition() { Height = GridLength.Auto },
                    new RowDefinition() { Height = GridLength.Auto },
                    new RowDefinition() { Height = GridLength.Auto },
                    new RowDefinition() { Height = GridLength.Auto },
                    new RowDefinition() { Height = GridLength.Auto },
                    new RowDefinition() { Height = GridLength.Auto },
                    new RowDefinition() { Height = GridLength.Auto },
                    new RowDefinition() { Height = GridLength.Auto },
                    new RowDefinition() { Height = GridLength.Auto },
                    new RowDefinition() { Height = GridLength.Auto }
                },
                VerticalOptions = LayoutOptions.Fill
            };

            //Row 1
            //Title
            Label titleLabel = new Label { FontSize = GlobalVars.sizeLarge, FontAttributes = FontAttributes.Bold };
            titleLabel.SetBinding(Label.TextProperty, "Title");
            wholePage.Children.Add(titleLabel, 0, 0);
            Grid.SetColumnSpan(titleLabel, 4);


            //Avl. Tickets
            StackLayout avlTicketHorizStack = new StackLayout { Orientation = StackOrientation.Horizontal, Padding = 0, Spacing = 0, HorizontalOptions = LayoutOptions.End };
            Label avlTicketsStatic = new Label { FontSize = GlobalVars.sizeMedium, Text = "Avl. Tickets: ", VerticalOptions = LayoutOptions.End };
            Label avlTicketsDynamic = new Label { FontSize = GlobalVars.sizeLarge, FontAttributes = FontAttributes.Bold, LineBreakMode = LineBreakMode.NoWrap, VerticalTextAlignment = TextAlignment.End };
            avlTicketsDynamic.SetBinding(Label.TextProperty, "AvailableTickets");
            avlTicketHorizStack.Children.Add(avlTicketsStatic);
            avlTicketHorizStack.Children.Add(avlTicketsDynamic);
            wholePage.Children.Add(avlTicketHorizStack, 4, 0);
            Grid.SetColumnSpan(avlTicketHorizStack, 2);


            //Row 2
            //GroupCompany
            Label groupCompanyLabel = new Label { FontSize = GlobalVars.sizeMedium, Margin = new Thickness(10, 0, 0, 0) };
            groupCompanyLabel.SetBinding(Label.TextProperty, "GroupCompany");
            wholePage.Children.Add(groupCompanyLabel, 0, 1);
            Grid.SetColumnSpan(groupCompanyLabel, 4);


            //FormattedPlayers
            Label playerNumLabel = new Label { FontSize = GlobalVars.sizeMedium, HorizontalTextAlignment = TextAlignment.End };
            playerNumLabel.SetBinding(Label.TextProperty, "FormattedPlayers");
            wholePage.Children.Add(playerNumLabel, 4, 1);
            Grid.SetColumnSpan(playerNumLabel, 2);


            //Row 3
            //EventID
            Label eventIDLabel = new Label { FontSize = GlobalVars.sizeMedium, TextColor = GlobalVars.colorLink };
            eventIDLabel.SetBinding(Label.TextProperty, "ID");
            wholePage.Children.Add(eventIDLabel, 0, 2);
            Grid.SetColumnSpan(eventIDLabel, 4);

            eventIDLabel.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command<Label>((Label label) =>
                {
                    string newURL = ((GenEvent)this.BindingContext).LiveURL;

                    //string args = label.Text;

                    //string newArgs = args.Substring(
                    //                args.Length - Math.Min(5, args.Length)
                    //            );

                    //if (newArgs.StartsWith("0"))
                    //{
                    //    newArgs = args.Substring(
                    //                args.Length - Math.Min(6, args.Length)
                    //            );
                    //}

                    //CrossShare.Current.OpenBrowser(String.Format("https://www.gencon.com/events/{0}", newArgs), null);
                    // CrossShare.Current.OpenBrowser(newURL, null);
                    CrossShare.Current.OpenBrowser(newURL, new Plugin.Share.Abstractions.BrowserOptions
                    {
                        ChromeShowTitle = true,
                        UseSafariReaderMode = true,
                        UseSafariWebViewController = true
                    });

                    //Device.OpenUri(
                    //    new Uri(
                    //        String.Format("https://www.gencon.com/events/{0}", 
                    //            args.Substring(
                    //                args.Length - Math.Min(5, args.Length)
                    //            )
                    //        )
                    //    )
                    //);
                }),
                CommandParameter = eventIDLabel
            });


            //MinimumAge
            Label minimumAgeLabel = new Label { FontSize = GlobalVars.sizeMedium, HorizontalTextAlignment = TextAlignment.End };
            minimumAgeLabel.SetBinding(Label.TextProperty, "MinimumAge");
            wholePage.Children.Add(minimumAgeLabel, 4, 2);
            Grid.SetColumnSpan(minimumAgeLabel, 2);


            //Row 4
            //EventType
            Label eventTypeLabel = new Label { FontSize = GlobalVars.sizeMedium };
            eventTypeLabel.SetBinding(Label.TextProperty, "EventType");
            wholePage.Children.Add(eventTypeLabel, 0, 3);
            Grid.SetColumnSpan(eventTypeLabel, 4);


            //Cost
            Label costLabel = new Label { FontSize = GlobalVars.sizeLarge, FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.End, LineBreakMode = LineBreakMode.NoWrap };
            costLabel.SetBinding(Label.TextProperty, "FormattedCost");
            wholePage.Children.Add(costLabel, 4, 3);
            Grid.SetColumnSpan(costLabel, 2);


            //Row 5
            //FormattedDate
            Label formattedDateLabel = new Label { FontSize = GlobalVars.sizeMedium, Margin = new Thickness(0, 1, 0, 0) };
            formattedDateLabel.SetBinding(Label.TextProperty, "FormattedDate");
            wholePage.Children.Add(formattedDateLabel, 0, 4);
            Grid.SetColumnSpan(formattedDateLabel, 6);


            //Row 6
            //Location
            Label locationLabel = new Label { FontSize = GlobalVars.sizeMedium, Margin = new Thickness(10, 0, 0, 0), TextColor = GlobalVars.colorLink };
            locationLabel.SetBinding(Label.TextProperty, "Location");
            wholePage.Children.Add(locationLabel, 0, 5);
            Grid.SetColumnSpan(locationLabel, 6);

            locationLabel.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command<Label>((Label label) =>
                {
                    DetailChoice navigationLocation = GlobalVars.GetMapName(((GenEvent)this.BindingContext).Location);

                    if (navigationLocation != null)
                    {
                        if (navigationLocation.data.ToLower().StartsWith("http:") || navigationLocation.data.ToLower().StartsWith("https:"))
                        {
                            CrossShare.Current.OpenBrowser(navigationLocation.data, null);
                        }
                        else
                        {
                            Page page = (Page)Activator.CreateInstance(typeof(MapViewPage));
                            page.BindingContext = navigationLocation;
                            this.Navigation.PushAsync(page);
                        }
                    }

                }),
                CommandParameter = locationLabel
            });


            //Row 7
            //Description static
            Label descriptionStaticLabel = new Label { FontSize = GlobalVars.sizeMedium, FontAttributes = FontAttributes.Bold, Text = "Description:", Margin = new Thickness(0, 1, 0, 0) };
            wholePage.Children.Add(descriptionStaticLabel, 0, 6);
            Grid.SetColumnSpan(descriptionStaticLabel, 6);


            //Row 8
            //Description
            Label descriptionLabel = new Label { FontSize = GlobalVars.sizeMedium, Margin = new Thickness(10, 0, 0, 0) };
            descriptionLabel.SetBinding(Label.TextProperty, "Description");
            wholePage.Children.Add(descriptionLabel, 0, 7);
            Grid.SetColumnSpan(descriptionLabel, 6);


            //Row 9
            //Long Description static
            Label longDescriptionStaticLabel = new Label { FontSize = GlobalVars.sizeMedium, FontAttributes = FontAttributes.Bold, Text = "Long Description:", Margin = new Thickness(0, 1, 0, 0) };
            wholePage.Children.Add(longDescriptionStaticLabel, 0, 8);
            Grid.SetColumnSpan(longDescriptionStaticLabel, 6);


            //Row 10
            //Long Description
            Label longDescriptionLabel = new Label { FontSize = GlobalVars.sizeMedium, Margin = new Thickness(10, 0, 0, 0) };
            longDescriptionLabel.SetBinding(Label.TextProperty, "LongDescription");
            wholePage.Children.Add(longDescriptionLabel, 0, 9);
            Grid.SetColumnSpan(longDescriptionLabel, 6);


            //Row 11
            //Game System static
            Label gameSystemStaticLabel = new Label { FontSize = GlobalVars.sizeMedium, FontAttributes = FontAttributes.Bold, Text = "Game System:", Margin = new Thickness(0, 1, 0, 0) };
            wholePage.Children.Add(gameSystemStaticLabel, 0, 10);
            Grid.SetColumnSpan(gameSystemStaticLabel, 3);


            //Rules Edition static
            Label rulesEditionStaticLabel = new Label { FontSize = GlobalVars.sizeMedium, FontAttributes = FontAttributes.Bold, Text = "Rules Edition:", Margin = new Thickness(0, 1, 0, 0) };
            wholePage.Children.Add(rulesEditionStaticLabel, 3, 10);
            Grid.SetColumnSpan(rulesEditionStaticLabel, 3);


            //Row 12
            //Game System 
            Label gameSystemLabel = new Label { FontSize = GlobalVars.sizeMedium, Margin = new Thickness(10, 0, 0, 0) };
            gameSystemLabel.SetBinding(Label.TextProperty, "GameSystem");
            wholePage.Children.Add(gameSystemLabel, 0, 11);
            Grid.SetColumnSpan(gameSystemLabel, 3);


            //Rules Edition static
            Label rulesEditionLabel = new Label { FontSize = GlobalVars.sizeMedium, Margin = new Thickness(10, 0, 0, 0) };
            rulesEditionLabel.SetBinding(Label.TextProperty, "RulesEdition");
            wholePage.Children.Add(rulesEditionLabel, 3, 11);
            Grid.SetColumnSpan(rulesEditionLabel, 3);


            //Row 13
            //Experience Required static
            Label experienceRequiredStaticLabel = new Label { FontSize = GlobalVars.sizeMedium, FontAttributes = FontAttributes.Bold, Text = "Experience Required:", Margin = new Thickness(0, 1, 0, 0) };
            wholePage.Children.Add(experienceRequiredStaticLabel, 0, 12);
            Grid.SetColumnSpan(experienceRequiredStaticLabel, 3);


            //Materials Provided static
            Label materialsProvidedStaticLabel = new Label { FontSize = GlobalVars.sizeMedium, FontAttributes = FontAttributes.Bold, Text = "Materials Provided:", Margin = new Thickness(0, 1, 0, 0) };
            wholePage.Children.Add(materialsProvidedStaticLabel, 3, 12);
            Grid.SetColumnSpan(materialsProvidedStaticLabel, 3);


            //Row 14
            //Experience Required
            Label experienceRequiredLabel = new Label { FontSize = GlobalVars.sizeMedium, Margin = new Thickness(10, 0, 0, 0) };
            experienceRequiredLabel.SetBinding(Label.TextProperty, "ExperienceRequired");
            wholePage.Children.Add(experienceRequiredLabel, 0, 13);
            Grid.SetColumnSpan(experienceRequiredLabel, 3);


            //Materials Provided
            Label materialsProvidedLabel = new Label { FontSize = GlobalVars.sizeMedium, Margin = new Thickness(10, 0, 0, 0) };
            materialsProvidedLabel.SetBinding(Label.TextProperty, "MaterialsProvided");
            wholePage.Children.Add(materialsProvidedLabel, 3, 13);
            Grid.SetColumnSpan(materialsProvidedLabel, 3);


            //Row 15
            //Tournament static
            Label tournamentStaticLabel = new Label { FontSize = GlobalVars.sizeMedium, FontAttributes = FontAttributes.Bold, Text = "Tournament:", Margin = new Thickness(0, 1, 0, 0) };
            wholePage.Children.Add(tournamentStaticLabel, 0, 14);
            Grid.SetColumnSpan(tournamentStaticLabel, 3);


            //GMs static
            Label gmsStaticLabel = new Label { FontSize = GlobalVars.sizeMedium, FontAttributes = FontAttributes.Bold, Text = "GM(s):", Margin = new Thickness(0, 1, 0, 0) };
            wholePage.Children.Add(gmsStaticLabel, 3, 14);
            Grid.SetColumnSpan(gmsStaticLabel, 3);


            //Row 16
            //Tournament
            Label tournamentLabel = new Label { FontSize = GlobalVars.sizeMedium, Margin = new Thickness(10, 0, 0, 0) };
            tournamentLabel.SetBinding(Label.TextProperty, "Tournament");
            wholePage.Children.Add(tournamentLabel, 0, 15);
            Grid.SetColumnSpan(tournamentLabel, 3);


            //GMs
            Label gmsLabel = new Label { FontSize = GlobalVars.sizeMedium, Margin = new Thickness(10, 0, 0, 0) };
            gmsLabel.SetBinding(Label.TextProperty, "GMs");
            wholePage.Children.Add(gmsLabel, 3, 15);
            Grid.SetColumnSpan(gmsLabel, 3);


            //Row 17
            //Prerequisite static
            Label prerequisiteStaticLabel = new Label { FontSize = GlobalVars.sizeMedium, FontAttributes = FontAttributes.Bold, Text = "Prerequisite:", Margin = new Thickness(0, 1, 0, 0) };
            wholePage.Children.Add(prerequisiteStaticLabel, 0, 16);
            Grid.SetColumnSpan(prerequisiteStaticLabel, 6);


            //Row 18
            //Prerequisite
            Label prerequisiteLabel = new Label { FontSize = GlobalVars.sizeMedium, Margin = new Thickness(10, 0, 0, 0) };
            prerequisiteLabel.SetBinding(Label.TextProperty, "Prerequisite");
            wholePage.Children.Add(prerequisiteLabel, 0, 17);
            Grid.SetColumnSpan(prerequisiteLabel, 6);


            //Row 19
            //Web address static
            Label webAddressStaticLabel = new Label { FontSize = GlobalVars.sizeMedium, FontAttributes = FontAttributes.Bold, Text = "Web Address For More Info:", Margin = new Thickness(0, 1, 0, 0) };
            wholePage.Children.Add(webAddressStaticLabel, 0, 18);
            Grid.SetColumnSpan(webAddressStaticLabel, 6);


            //Row 20
            //Web address
            Label webAddressLabel = new Label { FontSize = GlobalVars.sizeMedium, Margin = new Thickness(10, 0, 0, 0), TextColor = GlobalVars.colorLink };
            webAddressLabel.SetBinding(Label.TextProperty, "WebAddressMoreInfo");
            wholePage.Children.Add(webAddressLabel, 0, 19);
            Grid.SetColumnSpan(webAddressLabel, 6);

            webAddressLabel.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command<Label>((Label label) =>
                {
                    string args = label.Text;

                    CrossShare.Current.OpenBrowser(args.StartsWith("http") ? args : "http://" + args, null);
                    //Device.OpenUri(
                    //    new Uri(
                    //        args.StartsWith("http") ? args : "http://" + args
                    //    )
                    //);
                }),
                CommandParameter = webAddressLabel
            });


            //Row 21
            //Email address static
            Label emailAddressStaticLabel = new Label { FontSize = GlobalVars.sizeMedium, FontAttributes = FontAttributes.Bold, Text = "Email Address For More Info:", Margin = new Thickness(0, 1, 0, 0) };
            wholePage.Children.Add(emailAddressStaticLabel, 0, 20);
            Grid.SetColumnSpan(emailAddressStaticLabel, 6);
            
            //Row 22
            //Web address
            Label emailAddressLabel = new Label { FontSize = GlobalVars.sizeMedium, Margin = new Thickness(10, 0, 0, 0), TextColor = GlobalVars.colorLink };
            emailAddressLabel.SetBinding(Label.TextProperty, "EmailAddressMoreInfo");
            wholePage.Children.Add(emailAddressLabel, 0, 21);
            Grid.SetColumnSpan(emailAddressLabel, 6);

            emailAddressLabel.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command<Label>((Label label) =>
                {
                    string args = label.Text;
                    Device.OpenUri(
                        new Uri(
                                "mailto:" + args.Trim()
                            )
                        );
                }),
                CommandParameter = emailAddressLabel
            });


            //Row 23
            //LastUpdatedMessage
            Label lastUpdatedLabel = new Label { FontSize = GlobalVars.sizeSmall, FontAttributes = FontAttributes.Italic, Margin = new Thickness(10, 1, 0, 0) };
            lastUpdatedLabel.SetBinding(Label.TextProperty, "FormattedUpdateTime");
            wholePage.Children.Add(lastUpdatedLabel, 0, 22);
            Grid.SetColumnSpan(lastUpdatedLabel, 6);

            wholePageScroller.Content = wholePage;

            popupHolder = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Padding = new Thickness(20, 20, 20, 20),
                BackgroundColor = Color.White
            };

            var entryLabel = new Label
            {
                Text = "Select a list from below: "
            };

            popupHolder.Children.Add(entryLabel);

            userListPicker = new ListView()
            {
                HeightRequest = 200
            };

            userListPicker.ItemTapped += UserEventListItem_Tapped;

            popupHolder.Children.Add(userListPicker);

            var buttonHolder = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Padding = new Thickness(10, 10, 10, 10),
            };

            //Button OKButton = new Button()
            //{
            //    Text = "OK"
            //};

            //OKButton.Clicked += OKButton_Clicked;

            Button CancelButton = new Button()
            {
                Text = "Cancel"
            };

            CancelButton.Clicked += Cancel_Clicked;

            //buttonHolder.Children.Add(OKButton);
            buttonHolder.Children.Add(CancelButton);

            popupHolder.Children.Add(buttonHolder);

            popupHolder.IsVisible = false;

            wholePageHolder.Children.Add(popupHolder);

            AbsoluteLayout.SetLayoutBounds(wholePageHolder, new Rectangle(0, 0, 1, 1));
            AbsoluteLayout.SetLayoutFlags(wholePageHolder, AbsoluteLayoutFlags.All);

            wholePageHolder.Children.Add(wholePageScroller);
            this.BackgroundColor = Color.White;
            
            // changing to popup, need to rearrange this a bit

            // top section, grid layout, with toolbar buttons [Calendar] [List] [Share] [close]
            // bottom section .. actual content
            var buttons = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition{Width = GridLength.Star},
                    new ColumnDefinition{Width = GridLength.Star},
                    new ColumnDefinition{Width = GridLength.Star},
                    new ColumnDefinition{Width = GridLength.Star}
                },
                RowDefinitions = new RowDefinitionCollection
                {
                    new RowDefinition{Height = 25}
                },
                Children =
                {
                    { new CalendarToolbarItem(),           0, 0},
                    { new AddEventToListToolbarItem(this), 1, 0},
                    { new ShareEventToolbarItem(this),     2, 0},
                    { new CloseEventPageToolbarItem(),     3, 0}
                }
            };

            var actualContent = new StackLayout
            {
                Children =
                {
                    buttons,
                    wholePageHolder

                }
            };

            this.Content = actualContent;
        }

        private void Cancel_Clicked(object sender, EventArgs e)
        {
            popupHolder.IsVisible = false;
        }

        private void UserEventListItem_Tapped(object sender, ItemTappedEventArgs e)
        {
            GenEvent currentEvent = (GenEvent)this.BindingContext; 

            if (e.Item != null)
            {
                try
                {
                    var selectedList = (string)e.Item;
                    if (!string.IsNullOrEmpty(selectedList))
                    {
                        Task.Factory.StartNew(async () =>
                        {
                            if (selectedList == newListString)
                            {
                                selectedList = "My New List";
                                var newList = await GlobalVars.db.AddUserEventList(selectedList);
                                newList.Events.Add(currentEvent);
                                newList.HasEventListChangedSinceSync = true;
                                await GlobalVars.db.UpdateUserEventListWithChildrenAsync(newList);
                                GlobalVars.View_GenUserListView.IsUpdateRequested = true;
                                //selectedList = newList.Title;
                            }
                            else
                            {
                                var currentList = await GlobalVars.db.GetUserEventListWithChildrenAsync(userEventLists.FirstOrDefault(d => d.Title == selectedList));
                                if (currentList != null)
                                {
                                    currentList.Events.Add(currentEvent);
                                    currentList.Events = currentList.Events.Distinct().OrderBy(d => d.StartDateTime).ToList();
                                    currentList.HasEventListChangedSinceSync = true;
                                    await GlobalVars.db.UpdateUserEventListWithChildrenAsync(currentList);
                                    GlobalVars.View_GenUserListView.IsUpdateRequested = true;
                                }
                            }

                            //Device.BeginInvokeOnMainThread(() =>
                            //{
                            GlobalVars.DoToast($"Added \"{currentEvent.ID}\" to list.", GlobalVars.ToastType.Green);
                            //});
                        });
                    }
                }
                catch (Exception ex)
                {
                    string fart = ex.Message;
                }
            }

            popupHolder.IsVisible = false;
        }

        private void OpenAddToListPrompt()
        {
            GenEvent currentEvent = (GenEvent)this.BindingContext;

            userEventLists = GlobalVars.db.UserEventLists;
            userListPicker.ItemsSource = UserListsTitles;

            popupHolder.IsVisible = true;
        }

        //internal void AddToUserEventList(GenEvent copyEvent, string selectedItemString, UserEventList selectedList)
        //{
            
        //    if (copyEvent != null && selectedItemString != null)
        //    {

        //        Task.Factory.StartNew(async () =>
        //        {
        //            var copy2Event = copyEvent;
        //            if (selectedItemString == ListSelectionModal.newListString)
        //            {
        //                selectedItemString = "My New List";
        //                var newList = await GlobalVars.db.AddUserEventList(selectedItemString);
        //                newList.Events.Add(copy2Event);
        //                await GlobalVars.db.UpdateUserEventListWithChildrenAsync(newList);
        //                selectedItemString = newList.Title;
        //            }
        //            else
        //            {
        //                var currentList = await GlobalVars.db.GetUserEventListWithChildrenAsync(selectedList);
        //                if (currentList != null)
        //                {
        //                    currentList.Events.Add(copy2Event);
        //                    currentList.Events = currentList.Events.Distinct().OrderBy(d => d.StartDateTime).ToList();
        //                    await GlobalVars.db.UpdateUserEventListWithChildrenAsync(currentList);
        //                }
        //            }
        //        });
        //    }

        //    Task.Factory.StartNew(() =>
        //    {
        //        PopupNavigation.Instance.PopAllAsync();
        //    });


        //    Device.BeginInvokeOnMainThread(() =>
        //    {
        //        GlobalVars.DoToast($"Added \"{copyEvent.ID}\" to list.", GlobalVars.ToastType.Green);
        //    });


        //}

        private void CalculatePaddingAmount()
        {
            paddingAmount = DependencyService.Get<ISafeAreaInsets>().Padding();

            if (paddingAmount.Left > 0 && paddingAmount.Right > 0)
            {
                paddingAmount.Top = 3;
                paddingAmount.Bottom = 3;
            }
            else
            {
                paddingAmount = new Thickness(3, 3, 3, 3);
            }
        }

        private void DeviceRotated(object sender, PageOrientationEventArgs e)
        {
            CalculatePaddingAmount();

            wholePageScroller.Padding = paddingAmount;
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            var eventItem = BindingContext as GenEvent;
            if (eventItem != null && eventItem.HasUpdateNotifications)
            {
                var changePopup = new EventChangeLogFull();
                changePopup.BindingContext = eventItem;
                Navigation.PushModalAsync(changePopup);
            }
        }
    }
}
