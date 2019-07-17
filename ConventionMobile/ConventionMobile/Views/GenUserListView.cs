using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ConventionMobile.Data;
using ConventionMobile.Model;
using ConventionMobile.Pages;
using Plugin.Share;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace ConventionMobile.Views
{
    public class GenUserListView : GenContentView
    {
        private AbsoluteLayout outerContainer;
        private Thickness paddingAmount = new Thickness(0, 0, 0, 0);
        private Picker eventListPicker;
        private List<UserEventList> userLists = new List<UserEventList>();
        private GenEventList genEventList;
        public ListView genEventListView;
        private ListView loadingListView;
        private ActivityIndicator loadingIndicator;
        private StackLayout eventDisplayWrapper;
        private StackLayout eventDisplay;
        private StackLayout eventEmptyListDisplay;

        private enum TextEntryMode { AddMode, RenameMode };
        private TextEntryMode CurrentMode;

        private Entry listEntryEntry;

        private StackLayout textEntryModal;
        private Label listEntryLabel;

        private StackLayout eventListManagementPane;
        private StackLayout selectedItemManagementPane;

        private ListView copyToListView;
        private StackLayout copyToListPopup;
        private Label copyToListLabel;

        private StackLayout shareListPopup;
        private Button shareListShareIntentButton;
        private Button shareListUpdateOnlineButton;
        private Button shareListCloseButton;
        private Label shareListMessageLabel;
        private Label shareListLinkLabel;

        //private List<GenEvent> genListSource = new List<GenEvent>();

        private UserEventList currentlySelectedList = null;

        private Image addListButton;
        private Image deleteListButton;
        private Image renameListButton;
        private Image deselectSelectedItemsButton;

        private Image copySelectedItemsButton;
        private Image deleteSelectedItemsButton;
        private Label selectedItemCountLabel;

        private DataTemplate itemTemplate;

        private bool isShowingEventList = false;

        private bool isInMultiSelectMode = false;

        private ObservableCollectionEx<GenEvent> onScreenObservableEventsCollection = new ObservableCollectionEx<GenEvent>();

        private bool isObservableCollectionEventsBound = false;

        private CancellationTokenSource updateCTS = null;

        private ToolbarItem shareListToolbarItem;

        private readonly GenMainPage _parentPage;

        public GenUserListView(GenMainPage parentPage) : base(GlobalVars.userListsTitle)
        {
            GlobalVars.View_GenUserListView = null;
            _parentPage = parentPage;

            itemTemplate = new DataTemplate(typeof(GenEventSelectableCell));
            itemTemplate.CreateContent();

            outerContainer = new AbsoluteLayout
            {
                Padding = 0
            };

            StackLayout wholePage = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Padding = 0,
                Spacing = 0,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };


            // START OF THE EVENT LIST MANAGEMENT PANE (SHOWN BY DEFAULT)
            eventListManagementPane = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Padding = 1,
                Spacing = 0,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            addListButton = new Image
            {
                Aspect = Aspect.AspectFit,
                HorizontalOptions = LayoutOptions.End,
                Source = ImageSource.FromFile("add_list_plus.png")
            };

            TapGestureRecognizer addListTap = new TapGestureRecognizer();
            addListTap.Tapped += AddList_Tapped;

            addListButton.GestureRecognizers.Add(addListTap);

            renameListButton = new Image
            {
                Aspect = Aspect.AspectFit,
                HorizontalOptions = LayoutOptions.End,
                Source = ImageSource.FromFile("rename_list.png")
            };

            TapGestureRecognizer renameListTap = new TapGestureRecognizer();
            renameListTap.Tapped += RenameList_Tapped;

            renameListButton.GestureRecognizers.Add(renameListTap);

            deleteListButton = new Image
            {
                Aspect = Aspect.AspectFit,
                HorizontalOptions = LayoutOptions.End,
                Source = ImageSource.FromFile("trash.png")
            };

            TapGestureRecognizer deleteListTap = new TapGestureRecognizer();
            deleteListTap.Tapped += DeleteList_Tapped;

            deleteListButton.GestureRecognizers.Add(deleteListTap);

            eventListPicker = new Picker
            {
                Title = "Your event lists",
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            eventListPicker.SelectedIndexChanged += OnEventListPickerSelected;

            eventListManagementPane.Children.Add(addListButton);
            eventListManagementPane.Children.Add(renameListButton);
            eventListManagementPane.Children.Add(deleteListButton);
            eventListManagementPane.Children.Add(eventListPicker);

            wholePage.Children.Add(eventListManagementPane);

            //START OF THE SELECTED ITEM MANAGEMENT PANE (HIDDEN BY DEFAULT)
            selectedItemManagementPane = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Padding = 1,
                Spacing = 0,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                IsVisible = false
            };

            selectedItemCountLabel = new Label
            {
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalOptions = LayoutOptions.FillAndExpand,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                FontSize = 16,
                Margin = 10,
                Text = ""
            };

            copySelectedItemsButton = new Image
            {
                Aspect = Aspect.AspectFit,
                HorizontalOptions = LayoutOptions.Center,
                Source = ImageSource.FromFile("move_to_list.png")
            };

            TapGestureRecognizer copySelectedItemsTap = new TapGestureRecognizer();
            copySelectedItemsTap.Tapped += CopySelectedItems_Tapped;

            copySelectedItemsButton.GestureRecognizers.Add(copySelectedItemsTap);

            deleteSelectedItemsButton = new Image
            {
                Aspect = Aspect.AspectFit,
                HorizontalOptions = LayoutOptions.Center,
                Source = ImageSource.FromFile("trash.png")
            };

            TapGestureRecognizer deleteSelectedItemsTap = new TapGestureRecognizer();
            deleteSelectedItemsTap.Tapped += DeleteSelectedItems_Tapped;

            deleteSelectedItemsButton.GestureRecognizers.Add(deleteSelectedItemsTap);

            deselectSelectedItemsButton = new Image
            {
                Aspect = Aspect.AspectFit,
                HorizontalOptions = LayoutOptions.Center,
                Source = ImageSource.FromFile("ic_cancel_black_24dp.png")
            };

            TapGestureRecognizer deselectSelectedItemsTap = new TapGestureRecognizer();
            deselectSelectedItemsTap.Tapped += DeselectSelectedItems_Tapped;

            deselectSelectedItemsButton.GestureRecognizers.Add(deselectSelectedItemsTap);

            selectedItemManagementPane.Children.Add(selectedItemCountLabel);
            selectedItemManagementPane.Children.Add(copySelectedItemsButton);
            selectedItemManagementPane.Children.Add(deleteSelectedItemsButton);
            selectedItemManagementPane.Children.Add(deselectSelectedItemsButton);

            wholePage.Children.Add(selectedItemManagementPane);


            //START OF LOWER EVENT SECTION
            eventDisplayWrapper = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Padding = paddingAmount,
                Spacing = 0,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            eventDisplay = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Padding = 0,
                Spacing = 6,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            eventDisplay.Children.Add(new Label
            {
                Text = GlobalVars.userListsEmptyMessage,
                FontSize = 12,
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HorizontalTextAlignment = TextAlignment.Center
            });

            eventEmptyListDisplay = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Padding = 0,
                Spacing = 6,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            eventEmptyListDisplay.Children.Add(new Label
            {
                Text = GlobalVars.userListSingleEmptyMessage,
                FontSize = 12,
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HorizontalTextAlignment = TextAlignment.Center
            });

            genEventList = new GenEventList
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            // start here, and add the bookmarked multi-select-view functionality to a new class, copied from geneventlist

            genEventListView = new ListView(ListViewCachingStrategy.RecycleElement)
            {
                RowHeight = (int)GlobalVars.sizeListCellHeight,
                SeparatorVisibility = SeparatorVisibility.None,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            loadingListView = new ListView
            {
                RowHeight = (int)GlobalVars.sizeListCellHeight,
                SeparatorVisibility = SeparatorVisibility.None
            };

            loadingIndicator = new ActivityIndicator()
            {
                IsRunning = true,
                IsVisible = false,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.White,
                MinimumWidthRequest = 400,
                MinimumHeightRequest = 400
            };

            textEntryModal = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Padding = new Thickness(20, 20, 20, 20),
                BackgroundColor = Color.White,
                IsVisible = false
            };

            listEntryLabel = new Label
            {
                Text = "Creating new list - please give your new list a name:"
            };

            textEntryModal.Children.Add(listEntryLabel);

            listEntryEntry = new Entry
            {
                Text = ""
            };

            textEntryModal.Children.Add(listEntryEntry);

            StackLayout buttonHolder = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Padding = new Thickness(10, 10, 10, 10),
            };

            Button OKButton = new Button()
            {
                Text = "OK"
            };

            OKButton.Clicked += OKButton_Clicked;

            Button CancelButton = new Button()
            {
                Text = "Cancel"
            };

            CancelButton.Clicked += Cancel_Clicked;

            buttonHolder.Children.Add(OKButton);
            buttonHolder.Children.Add(CancelButton);

            textEntryModal.Children.Add(buttonHolder);


            eventDisplayWrapper.Children.Add(eventDisplay);

            wholePage.Children.Add(eventDisplayWrapper);

            wholePage.Children.Insert(0, textEntryModal);


            //START OF COPY-TO-LIST PICKER:
            copyToListPopup = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Padding = new Thickness(20, 20, 20, 20),
                BackgroundColor = Color.White
            };

            copyToListLabel = new Label
            {
                Text = ""
            };

            copyToListPopup.Children.Add(copyToListLabel);

            copyToListView = new ListView()
            {
                HeightRequest = 200
            };

            copyToListView.ItemTapped += CopyToListView_ItemTapped;

            copyToListPopup.Children.Add(copyToListView);

            StackLayout copyToListButtonHolder = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Padding = new Thickness(10, 10, 10, 10),
            };
            Button CancelCopyButton = new Button()
            {
                Text = "Cancel"
            };

            CancelCopyButton.Clicked += CancelCopy_Clicked;

            //buttonHolder.Children.Add(OKButton);
            copyToListButtonHolder.Children.Add(CancelCopyButton);

            copyToListPopup.Children.Add(copyToListButtonHolder);

            copyToListPopup.IsVisible = false;

            wholePage.Children.Insert(0, copyToListPopup);


            //START OF SHARE LIST POPUP
            shareListPopup = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Padding = new Thickness(3, 3, 3, 3),
                BackgroundColor = Color.White
            };

            var shareListScrollView = new ScrollView
            {
                Padding = 0,
                Margin = 0
            };

            shareListMessageLabel = new Label
            {
                Text = ""
            };

            shareListLinkLabel = new Label
            {
                Text = "",
                IsVisible = false
            };

            shareListPopup.Children.Add(shareListMessageLabel);
            shareListPopup.Children.Add(shareListLinkLabel);

            StackLayout shareListButtonHolder = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Padding = new Thickness(0, 0, 0, 0),
                Margin = 0,
                IsClippedToBounds = true,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            shareListShareIntentButton = new Button
            {
                Text = "Share",
                Margin = 1,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            shareListShareIntentButton.Clicked += ShareListShareIntentButton_Clicked;

            shareListUpdateOnlineButton = new Button
            {
                Text = "Sync Online",
                Margin = 1,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            shareListUpdateOnlineButton.Clicked += ShareListUpdateOnlineButton_Clicked;

            shareListCloseButton = new Button
            {
                Text = "Close",
                Margin = 1
            };

            shareListCloseButton.Clicked += ShareListCancelButton_Clicked;

            shareListButtonHolder.Children.Add(shareListShareIntentButton);
            shareListButtonHolder.Children.Add(shareListUpdateOnlineButton);

            shareListPopup.Children.Add(shareListButtonHolder);
            shareListPopup.Children.Add(shareListCloseButton);

            shareListPopup.IsVisible = false;

            wholePage.Children.Insert(0, shareListPopup);

            outerContainer.Children.Add(wholePage);
            outerContainer.Children.Add(loadingIndicator);

            //outerContainer.Children.Add(textEntryModal);

            AbsoluteLayout.SetLayoutBounds(wholePage, new Rectangle(0, 0, 1, 1));
            AbsoluteLayout.SetLayoutFlags(wholePage, AbsoluteLayoutFlags.All);

            //AbsoluteLayout.SetLayoutFlags(loadingLabel,
            //    AbsoluteLayoutFlags.PositionProportional);
            //AbsoluteLayout.SetLayoutBounds(loadingLabel,
            //    new Rectangle(0.5,
            //        0.5, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));

            AbsoluteLayout.SetLayoutFlags(loadingIndicator,
                AbsoluteLayoutFlags.PositionProportional);
            AbsoluteLayout.SetLayoutBounds(loadingIndicator,
                new Rectangle(0.5,
                              0.5, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));

            //AbsoluteLayout.SetLayoutFlags(textEntryModal,
            //    AbsoluteLayoutFlags.PositionProportional);
            //AbsoluteLayout.SetLayoutBounds(textEntryModal,
            //    new Rectangle(0.5,
            //                  0.5, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));

            shareListToolbarItem = new ToolbarItem("Share", "ic_share_black_24dp.png", () =>
            {
                OpenShareListPane(true);
            });

            UpdateUserLists();

            Content = outerContainer;

            this.OnAppearingHandler += GenUserListView_OnAppearingHandler;

            this.ToolbarItems.Add(new ToolbarItem("Font Size", "baseline_format_size_black_24.png", () =>
            {
                var page = new DisplayOptionsPage();
                PopupNavigation.Instance.PushAsync(page);
            }));

            this.ToolbarItems.Add(new ToolbarItem("Refresh", "ic_refresh_black_24dp.png", () =>
            {
                GlobalVars.View_GenEventsLoadingView.StartLoad();
            }));

            GlobalVars.View_GenUserListView = this;
        }

        /// <summary>
        /// This method fires in place of OnAppearing. 
        /// We need to check if another point in the program has altered the contents of this page and need it updated
        /// </summary>
        private void GenUserListView_OnAppearingHandler(object sender, EventArgs e)
        {
            try
            {
                if (IsUpdateRequested || GlobalVars.View_GenUserListView.IsUpdateRequested)
                {
                    IsUpdateRequested = false;
                    GlobalVars.View_GenUserListView.IsUpdateRequested = false;
                    UpdateUserLists();
                }
            }
            catch (Exception)
            {

            }
        }

        private void ShareListCancelButton_Clicked(object sender, EventArgs e)
        {
            SetShareListPopupVisibility(false);
        }

        private void ShareListUpdateOnlineButton_Clicked(object sender, EventArgs e)
        {
            Task.Factory.StartNew(async () =>
            {
                await UpdateSyncCurrentList();
            });
        }

        private void ShareListShareIntentButton_Clicked(object sender, EventArgs e)
        {
            if (currentlySelectedList != null && !string.IsNullOrEmpty(currentlySelectedList.ExternalAddress))
            {
                CrossShare.Current.Share(new Plugin.Share.Abstractions.ShareMessage
                {
                    Url = currentlySelectedList.PublicURL,
                    Text = $"Here's the link for my \"{currentlySelectedList.Title}\" customized event list for {GlobalVars.appTitle}!",
                    Title = $"My customized {GlobalVars.appTitle} event list"
                },
                new Plugin.Share.Abstractions.ShareOptions
                {
                    ChooserTitle = "Share Customized Event List"
                });
            }
        }

        private void SetShareListPopupVisibility(bool isVisible)
        {
            shareListPopup.IsVisible = isVisible;
            UpdateListIcons(true);
        }

        private void OpenShareListPane(bool toggleClosed = false)
        {
            if (!copyToListPopup.IsVisible)
            {
                if (toggleClosed && shareListPopup.IsVisible)
                {
                    SetShareListPopupVisibility(false);
                }
                else
                {
                    if (currentlySelectedList != null)
                    {
                        // if it's new to online, we need to present a "new share" type screen
                        if (string.IsNullOrEmpty(currentlySelectedList.InternalSecret))
                        {
                            shareListMessageLabel.Text = "Click \"Create Link\" to create a web link to your list.";
                            shareListLinkLabel.IsVisible = false;
                            shareListShareIntentButton.IsVisible = false;
                            shareListUpdateOnlineButton.Text = "Create Link";
                            shareListUpdateOnlineButton.IsVisible = true;
                        }
                        // otherwise already been synced online 
                        else
                        {
                            shareListShareIntentButton.IsVisible = true;

                            shareListUpdateOnlineButton.Text = "Update/Create Link";

                            shareListLinkLabel.Text = currentlySelectedList.PublicURL;
                            shareListLinkLabel.TextColor = GlobalVars.colorLink;
                            shareListLinkLabel.GestureRecognizers.Clear();
                            shareListLinkLabel.GestureRecognizers.Add(new TapGestureRecognizer
                            {
                                Command = new Command<Label>((Label label) =>
                                {
                                    string newURL = currentlySelectedList.PublicURL;
                                    CrossShare.Current.OpenBrowser(newURL, new Plugin.Share.Abstractions.BrowserOptions
                                    {
                                        ChromeShowTitle = true,
                                        UseSafariReaderMode = true,
                                        UseSafariWebViewController = true
                                    });
                                }),
                                CommandParameter = shareListLinkLabel
                            });

                            shareListLinkLabel.IsVisible = true;

                            if (currentlySelectedList.HasEventListChangedSinceSync)
                            {
                                shareListUpdateOnlineButton.IsVisible = true;
                                shareListMessageLabel.Text = "This list is online, but has changed since the last time you created a link. You can:"
                                    + "\n\n1) Share your list as it was when you last created a link (click \"Share\")"
                                    + "\n\n    - or -"
                                    + "\n\n2) Update your link with your changes, or create a brand new link (click \"Update/Create Link\")";
                            }
                            else
                            {
                                shareListMessageLabel.Text = "This list is online!";
                                shareListUpdateOnlineButton.IsVisible = false;

                            }
                        }
                        SetShareListPopupVisibility(true);
                    }
                }
            }
        }

        private async Task UpdateSyncCurrentList()
        {
            if (currentlySelectedList != null)
            {
                if (!string.IsNullOrEmpty(currentlySelectedList.ExternalAddress))
                {
                    string updateString = "Update Existing Link";
                    string createNewString = "Create A New Link";
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        var questionResult = await _parentPage.DisplayActionSheet("Which would you like to do?", "Cancel", null, updateString, createNewString);
                        if (questionResult == updateString)
                        {
                            await Task.Factory.StartNew(async () =>
                            {
                                UserEventList internetList = await App.GenEventManager.InsertUpdateUserEventListAsync(currentlySelectedList);
                                if (internetList != null)
                                {
                                    currentlySelectedList = internetList;
                                    await GlobalVars.db.UpdateUserEventListWithChildrenAsync(currentlySelectedList);

                                    GlobalVars.DoToast("Link updated!", GlobalVars.ToastType.Green, 3000);

                                    Device.BeginInvokeOnMainThread(() =>
                                    {
                                        OpenShareListPane();
                                    });
                                }
                                else
                                {
                                    GlobalVars.DoToast("Unfortunately, we were unable to sync the list online. Please check your internet connection or try again later.", GlobalVars.ToastType.Red, 5000);
                                }
                            });
                        }
                        else if (questionResult == createNewString)
                        {
                            await Task.Factory.StartNew(async () =>
                            {
                                UserEventList internetList = await App.GenEventManager.InsertUpdateUserEventListAsync(new UserEventList
                                {
                                    Title = currentlySelectedList.Title,
                                    Events = currentlySelectedList.Events,
                                    ID = currentlySelectedList.ID
                                });
                                if (internetList != null)
                                {
                                    currentlySelectedList = internetList;
                                    await GlobalVars.db.UpdateUserEventListWithChildrenAsync(currentlySelectedList);

                                    GlobalVars.DoToast("New Link Created!", GlobalVars.ToastType.Green, 3000);

                                    Device.BeginInvokeOnMainThread(() =>
                                    {
                                        OpenShareListPane();
                                    });
                                }
                                else
                                {
                                    GlobalVars.DoToast("Unfortunately, we were unable to sync the list online. Please check your internet connection or try again later.", GlobalVars.ToastType.Red, 5000);
                                }
                            });
                        }
                    });
                }
                else
                {
                    UserEventList internetList = await App.GenEventManager.InsertUpdateUserEventListAsync(new UserEventList
                    {
                        Title = currentlySelectedList.Title,
                        Events = currentlySelectedList.Events,
                        ID = currentlySelectedList.ID
                    });
                    if (internetList != null)
                    {
                        currentlySelectedList = internetList;
                        await GlobalVars.db.UpdateUserEventListWithChildrenAsync(currentlySelectedList);
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            OpenShareListPane();
                        });
                    }
                    else
                    {
                        GlobalVars.DoToast("Unfortunately, we were unable to sync the list online. Please check your internet connection or try again later.", GlobalVars.ToastType.Red, 5000);
                    }
                }


                ////show pop up window "generating link..." then generate a link. if the link already exists, show popup with a share button or generate new link or cancel
                //Task.Factory.StartNew(async () =>
                //{
                //    UserEventList internetList = await App.GenEventManager.InsertUpdateUserEventListAsync(currentlySelectedList);

                //    //Everything up until this point is working fine. Make something here that fails gracefully if internet isn't working.
                //    currentlySelectedList = internetList;

                //    Device.BeginInvokeOnMainThread(() =>
                //    {
                //        OpenShareListPane();
                //    });
                //});

                //DependencyService.Get<IShareExtensions>().ShareHTML(new Plugin.Share.Abstractions.ShareMessage
                //{
                //    Text = $"<!DOCTYPE html><html><body>I am sharing my custom event list with you. Make sure you have the {GlobalVars.appTitle} app installed before trying to open the link: <p>" + GlobalVars.GenerateUserEventListLinkHTML(currentlySelectedList) + "</p></body></html>",
                //    Title = GlobalVars.appTitle + " event list: " + currentlySelectedList.Title,
                //    //Url = $"<html>I am sharing my custom event list with you. Make sure you have the {GlobalVars.appTitle} app installed before trying to open the link: " + GlobalVars.GenerateUserEventListLinkHTML(currentlySelectedList) + "</html>",
                //},
                //new Plugin.Share.Abstractions.ShareOptions
                //{
                //    ChooserTitle = "Share My List"
                //});
            }
        }

        private void UpdateToolBarItems()
        {
            if (currentlySelectedList != null && currentlySelectedList.Events.Count > 0 && !copyToListPopup.IsVisible)
            {
                ToolbarItems.Remove(shareListToolbarItem);
                ToolbarItems.Add(shareListToolbarItem);
            }
            else
            {
                ToolbarItems.Remove(shareListToolbarItem);
            }
        }


        private void DeselectSelectedItems_Tapped(object sender, EventArgs e)
        {
            onScreenObservableEventsCollection.Where(d => d.IsSelected).ToList().ForEach(d => { d.IsSelected = false; });
            UpdateUserLists();
        }

        private void CancelCopy_Clicked(object sender, EventArgs e)
        {
            copyToListPopup.IsVisible = false;
        }

        private void CopyToListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item != null)
            {
                var copyItems = onScreenObservableEventsCollection.Where(d => d.IsSelected).ToList();

                var tappedListString = (string)e.Item;

                var tappedList = userLists.FirstOrDefault(d => d.Title == tappedListString);

                if (copyItems.Count > 0 && tappedList != null)
                {
                    Task.Factory.StartNew(async () =>
                    {
                        var copyToThisList = await GlobalVars.db.GetUserEventListWithChildrenAsync(tappedList);
                        if (copyToThisList != null)
                        {
                            copyToThisList.Events.AddRange(copyItems);
                            copyToThisList.Events = copyToThisList.Events.Distinct().OrderBy(d => d.StartDateTime).ToList();
                            copyToThisList.HasEventListChangedSinceSync = true;

                            await GlobalVars.db.UpdateUserEventListWithChildrenAsync(copyToThisList);

                            Device.BeginInvokeOnMainThread(() =>
                            {
                                GlobalVars.DoToast("Events added to list.", GlobalVars.ToastType.Green);
                            });
                        }
                    });
                }
            }

            copyToListPopup.IsVisible = false;
            onScreenObservableEventsCollection.Where(d => d.IsSelected).ToList().ForEach(d => { d.IsSelected = false; });
            UpdateUserLists();
        }

        private void OpenCopyToListPrompt()
        {
            if (UserListsTitlesWithoutCurrent.Count > 0)
            {
                copyToListView.ItemsSource = UserListsTitlesWithoutCurrent;
                var selectedCount = onScreenObservableEventsCollection.Where(d => d.IsSelected).Count();
                string plural = selectedCount > 1 ? "s" : "";
                copyToListLabel.Text = $"To which list do you want to copy your {selectedCount} event{plural}?: ";

                copyToListPopup.IsVisible = true;
            }
        }

        private void DeleteSelectedItems_Tapped(object sender, EventArgs e)
        {
            if (!copyToListPopup.IsVisible && !shareListPopup.IsVisible)
            {
                var selectedCount = onScreenObservableEventsCollection.Where(d => d.IsSelected).Count();

                //IS A LIST SELECTED IN EVENTLISTPICKER?
                if (selectedCount > 0)
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        //SHOW POP UP CONFIRMATION WINDOW ARE YOU SURE
                        string plural = selectedCount > 1 ? "s" : "";
                        var confirmDelete = await _parentPage.DisplayAlert("Confirmation", $"Are you sure you want to delete the selected {selectedCount} event{plural} from this list?", "Delete", "Cancel");
                        if (confirmDelete)
                        {
                            //IF USER TAPPED YES, DELETE THE SELECTED EVENTS FROM THE LIST AND COMMIT TO DATABASE
                            currentlySelectedList.Events = onScreenObservableEventsCollection.Where(d => !d.IsSelected).ToList();

                            currentlySelectedList.HasEventListChangedSinceSync = true;

                            onScreenObservableEventsCollection.Where(d => d.IsSelected).ToList().ForEach(d => { d.IsSelected = false; });

                            await GlobalVars.db.UpdateUserEventListWithChildrenAsync(currentlySelectedList);
                            //Device.BeginInvokeOnMainThread(() =>
                            //{
                            UpdateUserLists();
                            //});
                        }
                    });
                }
            }
        }

        private void CopySelectedItems_Tapped(object sender, EventArgs e)
        {
            if (!copyToListPopup.IsVisible && !shareListPopup.IsVisible)
            {
                OpenCopyToListPrompt();
            }
        }

        //private void GenEventListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        //{
        //    UpdateSelectionMode(false, (GenEvent)e.SelectedItem);
        //}

        private void Cancel_Clicked(object sender, EventArgs e)
        {
            textEntryModal.IsVisible = false;
        }

        private void OKButton_Clicked(object sender, EventArgs e)
        {
            // ADD IS HERE:
            string inputText = listEntryEntry.Text;

            if (!string.IsNullOrEmpty(inputText))
            {
                if (CurrentMode == TextEntryMode.AddMode)
                {
                    Task.Factory.StartNew(async () => { await AddUserList(inputText); });
                }
                else //otherwise we're renaming
                {
                    //WAIT FOR TEXT ENTRY TO CLOSE. IF USER ENTERED TEXT, UPDATE USERLISTPAGE TO DATABASE
                    if (!string.IsNullOrEmpty(inputText) && currentlySelectedList.Title != inputText)
                    {
                        currentlySelectedList.Title = inputText;
                        Task.Factory.StartNew(async () =>
                        {
                            currentlySelectedList.HasEventListChangedSinceSync = true;
                            var updatedList = await GlobalVars.db.UpdateUserEventListWithChildrenAsync(currentlySelectedList);
                            if (updatedList != null)
                            {
                                Device.BeginInvokeOnMainThread(() =>
                                {
                                    UpdateUserLists(updatedList.Title);
                                });
                            }
                        });
                    }
                }
            }

            textEntryModal.IsVisible = false;
        }

        private void RenameList_Tapped(object sender, EventArgs e)
        {
            if (eventListPicker.SelectedItem != null && !shareListPopup.IsVisible)
            {
                listEntryEntry.Text = currentlySelectedList.Title;
                listEntryLabel.Text = "Rename your list:";
                ShowTextEntryModal(TextEntryMode.RenameMode);
            }
        }

        private void DeleteList_Tapped(object sender, EventArgs e)
        {
            //IS A LIST SELECTED IN EVENTLISTPICKER?
            if (eventListPicker.SelectedItem != null && !shareListPopup.IsVisible)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    //SHOW POP UP CONFIRMATION WINDOW ARE YOU SURE
                    var confirmDelete = await _parentPage.DisplayAlert("Confirmation", $"Are you sure you want to delete your list \"{(string)eventListPicker.SelectedItem}\"?", "Delete", "Cancel");
                    if (confirmDelete)
                    {
                        //IF USER TAPPED YES, DELETE THE LIST FROM THE DATABASE AND UPDATE SCREEN
                        await GlobalVars.db.DeleteUserEventList(currentlySelectedList);
                        //Device.BeginInvokeOnMainThread(() =>
                        //{
                        UpdateUserLists();
                        //});
                    }
                });
            }
        }

        private void ShowTextEntryModal(TextEntryMode mode)
        {
            CurrentMode = mode;
            textEntryModal.IsVisible = true;
        }

        private void AddList_Tapped(object sender, EventArgs e)
        {
            if (!shareListPopup.IsVisible)
            {
                listEntryEntry.Text = "";
                listEntryLabel.Text = "Creating new list - please give your new list a name:";
                ShowTextEntryModal(TextEntryMode.AddMode);
            }
        }

        private async Task AddUserList(string userListTitle)
        {
            await GlobalVars.db.AddUserEventList(userListTitle);

            Device.BeginInvokeOnMainThread(() =>
            {
                UpdateUserLists();
            });
        }

        private void OnEventListPickerSelected(object sender, EventArgs e)
        {
            textEntryModal.IsVisible = false;
            SetShareListPopupVisibility(false);
            if (eventListPicker.SelectedItem != null)
            {
                Task.Factory.StartNew(async () =>
                {
                    currentlySelectedList = userLists.FirstOrDefault(d => d.Title == (string)eventListPicker.SelectedItem);
                    currentlySelectedList = await GlobalVars.db.GetUserEventListWithChildrenAsync(currentlySelectedList);
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        SetGenListContent();
                    });
                });
            }
        }

        private void DeviceRotated(object sender, PageOrientationEventArgs e)
        {
            CalculatePaddingAmount();

            eventDisplayWrapper.Padding = paddingAmount;
        }

        private void SetGenListContent()
        {
            loadingIndicator.IsVisible = false;


            if (currentlySelectedList != null)
            {

                if (this.updateCTS != null)
                {
                    this.updateCTS.Cancel();
                }

                if (currentlySelectedList.Events.Count == 0)
                {
                    isShowingEventList = false;
                    eventDisplayWrapper.Children.Remove(eventDisplay);
                    eventDisplayWrapper.Children.Remove(genEventList);
                    eventDisplayWrapper.Children.Remove(eventEmptyListDisplay);
                    eventDisplayWrapper.Children.Add(eventEmptyListDisplay);
                    UpdateToolBarItems();
                }
                else
                {
                    this.updateCTS = new CancellationTokenSource();
                    Task.Factory.StartNew(() =>
                    {
                        RunSetGenListContent(this.updateCTS.Token);
                    });
                }
            }
        }

        private void RunSetGenListContent(CancellationToken ct)
        {
            try
            {
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    if (!isShowingEventList)
                    {
                        isShowingEventList = true;
                        eventDisplayWrapper.Children.Remove(eventDisplay);
                        eventDisplayWrapper.Children.Remove(eventEmptyListDisplay);
                        eventDisplayWrapper.Children.Add(genEventList);
                    }
                    loadingIndicator.IsVisible = true;
                                                           

                    genEventListView.ItemTemplate = itemTemplate;

                    //ListView listView = new ListView
                    //{
                    //    ItemTemplate = itemTemplate
                    //};

                    Task.Factory.StartNew(() =>
                    {
                        try
                        {

                            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                            {
                                //var tempItemTemplate = new DataTemplate(typeof(TextCell));
                                //tempItemTemplate.CreateContent();

                                //loadingListView.ItemTemplate = tempItemTemplate;

                                //loadingListView.ItemsSource = new List<TextCell>()
                                //{
                                //    new TextCell()
                                //    {
                                //        Text =  "Loading..."
                                //    }
                                //};

                                //genEventList.Content = loadingListView;
                                if (isObservableCollectionEventsBound)
                                {
                                    onScreenObservableEventsCollection.OnLongPressChanged -= GenEventListViewItemsSource_LongPressChanged;
                                    onScreenObservableEventsCollection.OnIsTapped -= OnScreenObservableEventsCollection_OnIsTapped;
                                    isObservableCollectionEventsBound = false;
                                }
                                onScreenObservableEventsCollection = new ObservableCollectionEx<GenEvent>(currentlySelectedList.Events.OrderBy(d => d.StartDateTime).ToList());
                                onScreenObservableEventsCollection.OnLongPressChanged += GenEventListViewItemsSource_LongPressChanged;
                                onScreenObservableEventsCollection.OnIsTapped += OnScreenObservableEventsCollection_OnIsTapped;
                                isObservableCollectionEventsBound = true;
                                genEventListView.ItemsSource = onScreenObservableEventsCollection;
                                genEventList.Content = genEventListView;
                                loadingIndicator.IsVisible = false;
                                UpdateToolBarItems();
                                //dontCloseAutoComplete = true;
                            });

                        }
                        catch (Exception ex)
                        {
                            string error = ex.Message;
                        }
                    });
                });


                // genEventListView.ItemsSource = await GlobalVars.db.GetItemsFTS4(searchEntry.Text, getDBOptions());


            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
        }

        private void OnScreenObservableEventsCollection_OnIsTapped(object sender, TappedEventArgs e)
        {
            if (!textEntryModal.IsVisible)
            {
                if (isInMultiSelectMode)
                {
                    UpdateSelectionMode(false, (GenEvent)sender);
                }
                else
                {
                    GenEvent selectedEvent = (GenEvent)sender;

                    var page = (PopupPage)Activator.CreateInstance(typeof(GenEventFull));
                    page.BindingContext = selectedEvent;
                    //await this.Navigation.PushAsync(page);
                    PopupNavigation.Instance.PushAsync(page);
                }
            }
        }

        private void GenEventListViewItemsSource_LongPressChanged(object sender, GenEvent.LongPressChangedEventArgs e)
        {
            if (!textEntryModal.IsVisible && !copyToListPopup.IsVisible && !shareListPopup.IsVisible)
            {
                var item = onScreenObservableEventsCollection.FirstOrDefault(d => d.ID == e.genEvent.ID);
                if (item != null)
                {
                    //item.IsSelected = !item.IsSelected;
                    item._IsLongPressSelected = false;
                    e.genEvent._IsLongPressSelected = false;

                    UpdateSelectionMode(true, e.genEvent);
                }
            }
        }

        private void UpdateSelectionMode(bool sourceWasLongPress = false, GenEvent selectedItem = null)
        {
            if (!textEntryModal.IsVisible && !copyToListPopup.IsVisible && !shareListPopup.IsVisible)
            {
                var currentlySelected = onScreenObservableEventsCollection.Where(d => d.IsSelected);

                //careful when reviewing this code - "selectedItem.IsSelected" is going to be inverse for these if statements, since we apply update logic below
                if (!isInMultiSelectMode && sourceWasLongPress && (currentlySelected.Count() > 0 || (selectedItem != null && !selectedItem.IsSelected)))
                {
                    isInMultiSelectMode = true;
                }
                else if (currentlySelected.Count() == 0 && (selectedItem == null || (selectedItem != null && selectedItem.IsSelected)))
                {
                    isInMultiSelectMode = false;
                }

                if (isInMultiSelectMode)
                {
                    if (selectedItem != null)
                    {
                        var updateItem = onScreenObservableEventsCollection.FirstOrDefault(d => d.ID == selectedItem.ID);

                        updateItem.IsSelected = !updateItem.IsSelected;

                        genEventListView.SelectedItem = null;
                    }
                }

                UpdateListIcons();


            }
        }

        //private void GenEventListViewItemsSource_LongPressChanged()
        //{
        //    var allLongPressedItems = onScreenObservableEventsCollection.Where(d => d.IsLongPressSelected);
        //    foreach (var pressedItem in allLongPressedItems)
        //    {
        //        pressedItem.IsSelected = !pressedItem.IsSelected;
        //        pressedItem.IsLongPressSelected = false;
        //    }
        //}

        public void UpdateUserLists()
        {
            var innerCurrentlySelectedTitle = currentlySelectedList?.Title;

            userLists = GlobalVars.db.UserEventLists;
            eventListPicker.ItemsSource = UserListsTitles;

            if (UserListsTitles.Count > 0)
            {
                if (!string.IsNullOrEmpty(innerCurrentlySelectedTitle))
                {
                    var newSelectedIndex = UserListsTitles.IndexOf(innerCurrentlySelectedTitle);
                    if (newSelectedIndex > -1)
                    {
                        eventListPicker.SelectedIndex = newSelectedIndex;
                    }
                    else
                    {
                        eventListPicker.SelectedIndex = 0;
                    }
                }
                else
                {
                    eventListPicker.SelectedIndex = 0;
                }
            }
            else
            {
                isShowingEventList = false;
                eventDisplayWrapper.Children.Remove(eventDisplay);
                eventDisplayWrapper.Children.Remove(genEventList);
                eventDisplayWrapper.Children.Remove(eventEmptyListDisplay);
                eventDisplayWrapper.Children.Add(eventDisplay);
            }

            if (eventListPicker.SelectedIndex > -1)
            {
                SetGenListContent();
            }

            UpdateListIcons();
            UpdateToolBarItems();
        }

        private void UpdateUserLists(string selectedTitle)
        {
            UpdateUserLists();
            currentlySelectedList = userLists.FirstOrDefault(d => d.Title == selectedTitle);
            eventListPicker.SelectedItem = currentlySelectedList.Title;
        }

        private void UpdateListIcons(bool specialDirectCall = false)
        {
            if (shareListPopup.IsVisible && specialDirectCall) // the share list popup is visible
            {
                deleteListButton.IsVisible = false;
                renameListButton.IsVisible = false;
                addListButton.IsVisible = false;
            }
            else //otherwise act normal
            {
                addListButton.IsVisible = true;

                var selectedCount = 0;
                if (currentlySelectedList != null)
                {
                    selectedCount = onScreenObservableEventsCollection.Where(d => d.IsSelected).Count();
                }

                if (selectedCount > 0)
                {
                    string plural = selectedCount > 1 ? "s" : "";
                    selectedItemCountLabel.Text = $"Selected {selectedCount} item{plural}.";
                    eventListManagementPane.IsVisible = false;
                    selectedItemManagementPane.IsVisible = true;


                    if (UserListsTitlesWithoutCurrent.Count > 0)
                    {
                        copySelectedItemsButton.IsVisible = true;
                    }
                    else
                    {
                        copySelectedItemsButton.IsVisible = false;
                    }
                }
                else
                {
                    isInMultiSelectMode = false;
                    copyToListPopup.IsVisible = false;
                    shareListPopup.IsVisible = false;
                    eventListManagementPane.IsVisible = true;
                    selectedItemManagementPane.IsVisible = false;

                    if (eventListPicker.SelectedIndex > -1)
                    {
                        deleteListButton.IsVisible = true;
                        renameListButton.IsVisible = true;
                    }
                    else
                    {
                        deleteListButton.IsVisible = false;
                        renameListButton.IsVisible = false;
                    }
                }
            }


        }

        private List<string> UserListsTitles
        {
            get
            {
                return userLists.Select(d => d.Title).ToList();
            }
        }

        private List<string> UserListsTitlesWithoutCurrent
        {
            get
            {
                return userLists.Select(d => d.Title).Where(d => d != currentlySelectedList.Title).ToList();
            }
        }

        public bool IsUpdateRequested { get; set; } = false;

        private void CalculatePaddingAmount()
        {
            paddingAmount = DependencyService.Get<ISafeAreaInsets>().Padding();

            if (paddingAmount.Left > 0 && paddingAmount.Right > 0)
            {
                paddingAmount.Top = 0;
                paddingAmount.Bottom = 0;
            }
            else
            {
                paddingAmount = new Thickness(0, 0, 0, 0);
            }
        }
    }
}
