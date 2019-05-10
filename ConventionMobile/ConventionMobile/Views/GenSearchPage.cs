//using Plugin.Toasts;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using ConventionMobile.Data;
using System.Collections.Generic;
using System.Reactive.Linq;
using ConventionMobile.Model;
using System.Linq;
using MoreLinq;
using System.Collections.ObjectModel;
using ConventionMobile.Business;

namespace ConventionMobile.Views
{
    public class GenSearchPage : OrientationContentPage
    {
        private StackLayout eventDisplay;
        private StackLayout eventDisplayWrapper;
        
        private GenEventList genEventList;

        private Label lastUpdatedEventsLabel;
        private Label eventsTotalCountLabel;

        private Entry searchEntry;

        private Picker dayPicker;
        private Picker afterPicker;
        private Picker beforePicker;
        private Picker sortPicker;

        private SearchTerm searchTerm;

        private StackLayout autoCompleteHolder;
        private ListView matchListView;
        private ListView genEventListView;
        private ListView loadingListView;
        private ActivityIndicator loadingIndicator;
        //private Label loadingLabel;
        private Image changeSortButton;

        private Thickness paddingAmount = 0;

        private CancellationTokenSource searchCTS = null;

        private AbsoluteLayout outerContainer;

        private bool isShowingEventList = false;
        private ObservableCollection<GenEvent> AutoCompleteMatches = new ObservableCollection<GenEvent>();
        private int MatchCount = 0;

        private bool dontCloseAutoComplete = false;
        private bool dontUpdatePicker = false;

        private bool isSortDescending = true;
        
        //private bool ShowResult = false;

        private class DefaultSortChoice
        {
            public string Name { get; set; }
            public bool isSortDescending { get; set; }

            public DefaultSortChoice()
            {

            }

            public DefaultSortChoice(string Name, bool isSortDescending)
            {
                this.Name = Name;
                this.isSortDescending = isSortDescending;
            }
        }

        private List<DefaultSortChoice> defaultSortChoices = new List<DefaultSortChoice>()
        {
            new DefaultSortChoice("Time", false),
            new DefaultSortChoice("Title", false),
            new DefaultSortChoice("Ticket", true),
            new DefaultSortChoice("Price", false)
        };
                

        public GenSearchPage()
        {
            CalculatePaddingAmount();

            this.Title = GlobalVars.searchTitle;
            outerContainer = new AbsoluteLayout { Padding = 0 };

            StackLayout wholePage = new StackLayout { Orientation = StackOrientation.Vertical, Padding = 0, Spacing = 0, HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand };

            //StackLayout searchNavSectionTwo = new StackLayout { Orientation = StackOrientation.Horizontal, Padding = 0, Spacing = 0, HorizontalOptions = LayoutOptions.FillAndExpand };

            var searchNavSectionTwo = new Grid
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Padding = 1,
                ColumnSpacing = 1,
                ColumnDefinitions = new ColumnDefinitionCollection() {
                    new ColumnDefinition() { Width = new GridLength(4, GridUnitType.Star) },
                    new ColumnDefinition() { Width = new GridLength(4, GridUnitType.Star) },
                    new ColumnDefinition() { Width = new GridLength(4, GridUnitType.Star) },
                    new ColumnDefinition() { Width = new GridLength(3, GridUnitType.Star) },
                    new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) }
                },
                RowDefinitions = new RowDefinitionCollection() {
                    new RowDefinition() { Height = GridLength.Auto }
                }
            };

            //START OF UPPER SEARCH SECTION
            StackLayout searchNavSection = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Padding = 2,
                Spacing = 0,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
           
            searchEntry = new Entry
            {
                Keyboard = Keyboard.Create(0),
                Placeholder = "Search Here",
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            searchTerm = new SearchTerm();
            searchEntry.BindingContext = searchTerm;
            searchEntry.SetBinding(Entry.TextProperty, "searchTerm", BindingMode.TwoWay);

            searchEntry.Completed += (sender, e) =>
            {
                SetGenListContent();
            };

            //run the below on every keypress
            var clearWatcher = searchTerm.observer
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(final =>
            {
                searchTerm.isIgnoringNextEvent = false;
                outerContainer.Children.Remove(autoCompleteHolder);
            });

            //only run the below on legit text entry
            var autoCompleteWatcher = searchTerm.observer
                    .Throttle(TimeSpan.FromMilliseconds(300))            //Wait for the user to pause typing 
                    .Where(x => !string.IsNullOrEmpty(x) && x.Length > 2)  //Make sure the search term is long enough
                    .DistinctUntilChanged()
                    .ObserveOn(SynchronizationContext.Current)
                    .Subscribe((x) =>
            {
                Observable.FromAsync((_) => {
                    string term = x.Trim();
                    if ((term.Length == 10 || term.Length == 11) && GlobalVars.isTypedEventID(term))
                    {
                        Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                        {
                            SetGenListContent();
                        });
                        return new Task<IList<GenEvent>>(() =>
                        {
                            var returnMe = new List<GenEvent>();
                            return returnMe;
                        });
                    }
                    return GlobalVars.db.GetItemsFTS4(term, new GenconMobileDatabase.DBOptions[] { });
                })        //MatchOnEmail returns Task<IEnumerable<T>>
                    .TakeUntil(searchTerm.observer)                        //If seachterm changes abandon the current request
                    .ObserveOn(SynchronizationContext.Current)        //To be safe marshall response to the UI thread
                    .Subscribe(final =>                           //final holds the IEnumerable<T>    
                    {
                        try
                        {
                            if (!searchTerm.isIgnoringNextEvent)
                            {
                                AutoCompleteMatches.Clear();
                                if (final == null || !final.Any())
                                {
                                    MatchCount = 0;
                                    //ShowResult = false;
                                    RemoveAutoCompleteView();
                                }
                                else
                                {
                                    foreach (var g in final.DistinctBy(d => d.Title).OrderBy(d => d.Title).Take(5))
                                        AutoCompleteMatches.Add(g);
                                    MatchCount = AutoCompleteMatches.Count;
                                    matchListView.ItemsSource = AutoCompleteMatches;
                                    var r = AbsoluteLayout.GetLayoutBounds(autoCompleteHolder);
                                    r.Y = searchEntry.Y + (searchEntry.Height);
                                    r.Height = wholePage.Height;
                                    AbsoluteLayout.SetLayoutFlags(autoCompleteHolder, AbsoluteLayoutFlags.HeightProportional);
                                    AbsoluteLayout.SetLayoutBounds(autoCompleteHolder, r);
                                    //autoCompleteHolder.MinimumHeightRequest = wholePage.Height;
                                    matchListView.HeightRequest = searchEntry.Height * Math.Min(AutoCompleteMatches.Count, 3);
                                    //ShowResult = true;
                                    if (!outerContainer.Children.Contains(autoCompleteHolder))
                                    {
                                        outerContainer.Children.Add(autoCompleteHolder);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            string error = ex.Message;
                        }
                    });
            });


            var clearSearchButton = new Image { Aspect = Aspect.AspectFit, HorizontalOptions = LayoutOptions.End };
            clearSearchButton.Source = ImageSource.FromFile("ic_cancel_black_24dp.png");

            TapGestureRecognizer clearSearchTap = new TapGestureRecognizer();
            clearSearchTap.Tapped += ClearSearchTap_Tapped;

            clearSearchButton.GestureRecognizers.Add(clearSearchTap);

            var searchStartButton = new Image { Aspect = Aspect.AspectFit, HorizontalOptions = LayoutOptions.End };
            searchStartButton.Source = ImageSource.FromFile("ic_search_black_24dp.png");

            TapGestureRecognizer startSearchTap = new TapGestureRecognizer();
            startSearchTap.Tapped += StartSearchTap_Tapped;
            //startSearchTap.Tapped += DEBUGDEMO_Tapped;
            searchStartButton.GestureRecognizers.Add(startSearchTap);
            
            autoCompleteHolder = new StackLayout
            {
                Padding = 0,
                Spacing = 0,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Orientation = StackOrientation.Vertical,
                BackgroundColor = Color.FromRgba(0.2, 0.2, 0.2, 0.5)
            };
            
            var matchListTemplate = new DataTemplate(typeof(TextCell));
            matchListTemplate.SetBinding(TextCell.TextProperty, "Title");

            matchListView = new ListView
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Start,
                BindingContext = this,
                ItemTemplate = matchListTemplate,
                BackgroundColor = Color.White
            };
            
            autoCompleteHolder.Children.Add(matchListView);
            
            searchNavSection.Children.Add(searchEntry);
            searchNavSection.Children.Add(clearSearchButton);
            searchNavSection.Children.Add(searchStartButton);
            
            wholePage.Children.Add(searchNavSection);

            //searchNavSectionTwo
            dayPicker = new Picker
            {
                Title = "Days",
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            dayPicker.Items.Add("ALL");
            dayPicker.Items.Add("WED");
            dayPicker.Items.Add("THU");
            dayPicker.Items.Add("FRI");
            dayPicker.Items.Add("SAT");
            dayPicker.Items.Add("SUN");

            afterPicker = new Picker
            {
                Title = "After",
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            afterPicker.Items.Add("---");
            afterPicker.Items.Add("01:00AM");
            afterPicker.Items.Add("02:00AM");
            afterPicker.Items.Add("03:00AM");
            afterPicker.Items.Add("04:00AM");
            afterPicker.Items.Add("05:00AM");
            afterPicker.Items.Add("06:00AM");
            afterPicker.Items.Add("07:00AM");
            afterPicker.Items.Add("08:00AM");
            afterPicker.Items.Add("09:00AM");
            afterPicker.Items.Add("10:00AM");
            afterPicker.Items.Add("11:00AM");
            afterPicker.Items.Add("12:00PM");
            afterPicker.Items.Add("01:00PM");
            afterPicker.Items.Add("02:00PM");
            afterPicker.Items.Add("03:00PM");
            afterPicker.Items.Add("04:00PM");
            afterPicker.Items.Add("05:00PM");
            afterPicker.Items.Add("06:00PM");
            afterPicker.Items.Add("07:00PM");
            afterPicker.Items.Add("08:00PM");
            afterPicker.Items.Add("09:00PM");
            afterPicker.Items.Add("10:00PM");
            afterPicker.Items.Add("11:00PM");
            
            beforePicker = new Picker
            {
                Title = "Before",
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };


            beforePicker.Items.Add("---");
            beforePicker.Items.Add("01:00AM");
            beforePicker.Items.Add("02:00AM");
            beforePicker.Items.Add("03:00AM");
            beforePicker.Items.Add("04:00AM");
            beforePicker.Items.Add("05:00AM");
            beforePicker.Items.Add("06:00AM");
            beforePicker.Items.Add("07:00AM");
            beforePicker.Items.Add("08:00AM");
            beforePicker.Items.Add("09:00AM");
            beforePicker.Items.Add("10:00AM");
            beforePicker.Items.Add("11:00AM");
            beforePicker.Items.Add("12:00PM");
            beforePicker.Items.Add("01:00PM");
            beforePicker.Items.Add("02:00PM");
            beforePicker.Items.Add("03:00PM");
            beforePicker.Items.Add("04:00PM");
            beforePicker.Items.Add("05:00PM");
            beforePicker.Items.Add("06:00PM");
            beforePicker.Items.Add("07:00PM");
            beforePicker.Items.Add("08:00PM");
            beforePicker.Items.Add("09:00PM");
            beforePicker.Items.Add("10:00PM");
            beforePicker.Items.Add("11:00PM");

            sortPicker = new Picker
            {
                Title = "Sort",
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand
                
            };

            sortPicker.Items.Add("Time");
            sortPicker.Items.Add("Title");
            sortPicker.Items.Add("Ticket");
            sortPicker.Items.Add("Price");

            changeSortButton = new Image { Aspect = Aspect.AspectFit };
            changeSortButton.Source = ImageSource.FromFile("ic_arrow_downward_black_24dp.png");

            TapGestureRecognizer changeSortTap = new TapGestureRecognizer();
            changeSortTap.Tapped += ChangeSort_Tapped;

            changeSortButton.GestureRecognizers.Add(changeSortTap);

            searchNavSectionTwo.Children.Add(dayPicker, 0, 0);
            searchNavSectionTwo.Children.Add(afterPicker, 1, 0);
            searchNavSectionTwo.Children.Add(beforePicker, 2, 0);
            searchNavSectionTwo.Children.Add(sortPicker, 3, 0);
            searchNavSectionTwo.Children.Add(changeSortButton, 4, 0);

            wholePage.Children.Add(searchNavSectionTwo);

            //START OF LOWER EVENT SECTION
            eventDisplayWrapper = new StackLayout { Orientation = StackOrientation.Vertical, Padding = paddingAmount, Spacing = 0, HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand };
            eventDisplay = new StackLayout { Orientation = StackOrientation.Vertical, Padding = 0, Spacing = 6, HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand };

            lastUpdatedEventsLabel = new Label
            {
                Text = GlobalVars.eventsLastUpdatedPretty,
                FontSize = 10,
                FontAttributes = FontAttributes.Italic,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HorizontalTextAlignment = TextAlignment.Center
            };

            eventsTotalCountLabel = new Label
            {
                Text = GlobalVars.eventsTotalCountPretty,
                FontSize = 12,
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HorizontalTextAlignment = TextAlignment.Center
            };

            var thirdString = new Label
            {
                Text = GlobalVars.eventsExplanationPretty,
                FontSize = 10,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HorizontalTextAlignment = TextAlignment.Center
            };

            var showAllButton = new Button
            {
                Text = "SHOW ALL",
                HorizontalOptions = LayoutOptions.Center
            };

            var fourthString = new Label
            {
                Text = GlobalVars.eventsFinalInfoPretty,
                FontSize = 12,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HorizontalTextAlignment = TextAlignment.Center
            };

            eventDisplay.Children.Add(lastUpdatedEventsLabel);
            eventDisplay.Children.Add(eventsTotalCountLabel);
            eventDisplay.Children.Add(thirdString);
            eventDisplay.Children.Add(showAllButton);
            eventDisplay.Children.Add(fourthString);

            eventDisplayWrapper.Children.Add(eventDisplay);

            wholePage.Children.Add(eventDisplayWrapper);

            genEventList = new GenEventList
            {
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            genEventListView = new ListView(ListViewCachingStrategy.RecycleElement)
            {
                RowHeight = (int)GlobalVars.sizeListCellHeight,

                SeparatorVisibility = SeparatorVisibility.None
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

            outerContainer.Children.Add(wholePage);
            
            outerContainer.Children.Add(loadingIndicator);

            AbsoluteLayout.SetLayoutBounds(wholePage, new Rectangle(0, 0, 1, 1));
            AbsoluteLayout.SetLayoutFlags(wholePage, AbsoluteLayoutFlags.All);

            AbsoluteLayout.SetLayoutFlags(loadingIndicator,
                AbsoluteLayoutFlags.PositionProportional);
            AbsoluteLayout.SetLayoutBounds(loadingIndicator,
                new Rectangle(0.5,
                              0.5, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));

           

            Content = outerContainer;


            ToolbarItems.Add(new ToolbarItem("Refresh", "ic_refresh_black_24dp.png", () =>
            {
                
                //var homePage = ((App)Application.Current).HomePage;

                //homePage.overrideUpdateCheckEvents = true;
                //homePage.overrideUpdateCheckOptions = true;
                //Task.Factory.StartNew(homePage.CheckForNewEventsAsync);
                //GlobalVars.LoadingView.Start();
            }));

            //Start of hidden event portion RESUME CODING HERE

            showAllButton.Clicked += OnShowAllClicked;
            dayPicker.SelectedIndexChanged += OnDayPickerSelected;
            sortPicker.SelectedIndexChanged += OnSortPickerSelected;
            afterPicker.SelectedIndexChanged += OnAfterPickerSelected;
            beforePicker.SelectedIndexChanged += OnBeforePickerSelected;
            
            matchListView.ItemSelected += MatchListView_ItemSelected;
            //genEventListView.ItemTapped += GenEventListView_ItemTapped;

            genEventListView.ItemSelected += GenEventListView_ItemSelected;

            TapGestureRecognizer clearAutoCompleteTap = new TapGestureRecognizer();
            clearAutoCompleteTap.Tapped += ClearAutoCompleteTap_Tapped;

            autoCompleteHolder.GestureRecognizers.Add(clearAutoCompleteTap);

            this.LayoutChanged += GenNavigationPage_LayoutChanged;


            OnOrientationChanged += DeviceRotated;

        }

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

        private void DeviceRotated(object sender, PageOrientationEventArgs e)
        {
            CalculatePaddingAmount();

            eventDisplayWrapper.Padding = paddingAmount;
        }

        private void ChangeSort_Tapped(object sender, EventArgs e)
        {
            isSortDescending = !isSortDescending;
            if (isSortDescending)
            {
                changeSortButton.Source = ImageSource.FromFile("ic_arrow_downward_black_24dp.png");
            }
            else
            {
                changeSortButton.Source = ImageSource.FromFile("ic_arrow_upward_black_24dp.png");
            }

            if (!dontUpdatePicker) {
                SetGenListContent();
            }
        }

        private async void DEBUGDEMO_Tapped(object sender, EventArgs e)
        {
            FileManager fileManager = new FileManager(new FileService());

            fileManager.FileDownloadProgressUpdated += FileManager_FileDownloadProgressUpdated;

            List<bool> result = await fileManager.DownloadFiles(new List<string>() { "https://vectorlit.net/Default-Portrait.png" }, new List<string>() { "Default-Fart.png" });
        }

        private void FileManager_FileDownloadProgressUpdated(object sender, EventArgs e)
        {
            var args = ((FileDownloadUpdateEventArgs)e);
            //Device.BeginInvokeOnMainThread(() =>
            //{
                //bool returned = await GlobalVars.notifier.Notify(ToastNotificationType.Info,
                //    "File Progress updated", "Downloaded " + args.currentAmount.ToString() + "/" + args.totalAmount.ToString(), TimeSpan.FromSeconds(4));
                //var returned = await GlobalVars.notifier.Notify(new NotificationOptions()
                //{
                //    Title = "File Progress updated.",
                //    Description = "Downloaded " + args.currentAmount.ToString() + "/" + args.totalAmount.ToString()
                //});
                GlobalVars.DoToast("Downloaded " + args.currentAmount.ToString() + "/" + args.totalAmount.ToString(), GlobalVars.ToastType.Yellow);
            //});
        }

        private void StartSearchTap_Tapped(object sender, EventArgs e)
        {
            SetGenListContent();
        }

        private void GenNavigationPage_LayoutChanged(object sender, EventArgs e)
        {
            RemoveAutoCompleteView();
        }

        // private GenEvent selectedGenEvent = null;

        private async void GenEventListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            //// Deselect previous
            //if (selectedGenEvent != null)
            //{
            //    selectedGenEvent.SetColors(false);
            //}

            //// Select new
            //selectedGenEvent = (genEventListView.SelectedItem as GenEvent);
            //selectedGenEvent.SetColors(true);
            if (e.SelectedItem != null)
            {
                GenEvent selectedEvent = (GenEvent)e.SelectedItem;

                Page page = (Page)Activator.CreateInstance(typeof(GenEventFull));
                page.BindingContext = selectedEvent;
                await this.Navigation.PushAsync(page);
            }
        }

        //private async Task GenEventListView_ItemTapped(object sender, ItemTappedEventArgs e)
        //{
        //    GenEvent selectedEvent = (GenEvent)e.Item;

        //    Page page = (Page)Activator.CreateInstance(typeof(GenEventFull));
        //    page.BindingContext = selectedEvent;
        //    await this.Navigation.PushAsync(page);
        //}


        private void ClearSearchTap_Tapped(object sender, EventArgs e)
        {
            searchTerm.setSearchTermWithoutNotification("");

            searchEntry.SetBinding(Entry.TextProperty, "searchTerm", BindingMode.TwoWay);

            dontUpdatePicker = true;
            dayPicker.SelectedIndex = -1;
            sortPicker.SelectedIndex = -1;
            afterPicker.SelectedIndex = -1;
            beforePicker.SelectedIndex = -1;
            dontUpdatePicker = false;

            RemoveAutoCompleteView();
             
            SetGenListContent();
        }

        private void MatchListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (matchListView.SelectedItem != null)
            {
                searchTerm.setSearchTermWithoutNotification(((GenEvent)matchListView.SelectedItem).Title);

                searchEntry.SetBinding(Entry.TextProperty, "searchTerm", BindingMode.TwoWay);

                matchListView.SelectedItem = null;

                SetGenListContent();
            }

            RemoveAutoCompleteView();
        }

        private void ClearAutoCompleteTap_Tapped(object sender, EventArgs e)
        {
            RemoveAutoCompleteView();
        }
        

        private void RemoveAutoCompleteView()
        {
            if (!dontCloseAutoComplete)
            {
                outerContainer.Children.Remove(autoCompleteHolder);
            }
            else
            {
                dontCloseAutoComplete = false;
            }
        }

        private void OnDayPickerSelected(object sender, EventArgs e)
        {
            if (!dontUpdatePicker)
            {
                SetGenListContent();
            }
        }

        private void OnAfterPickerSelected(object sender, EventArgs e)
        {
            if (!dontUpdatePicker)
            {
                SetGenListContent();
            }
        }

        private void OnBeforePickerSelected(object sender, EventArgs e)
        {
            if (!dontUpdatePicker)
            {
                SetGenListContent();
            }
        }

        private void OnSortPickerSelected(object sender, EventArgs e)
        {
            isSortDescending = false;
            ChangeSort_Tapped(sender, e);
        }

        private void OnShowAllClicked(object sender, EventArgs e)
        {
            SetGenListContent();
        }

        private void SetGenListContent()
        {
            loadingIndicator.IsVisible = false;
            RemoveAutoCompleteView();
            if (this.searchCTS != null)
            {
                this.searchCTS.Cancel();
            }

            this.searchCTS = new CancellationTokenSource();
            Task.Factory.StartNew(() => {
                RunSetGenListContent(this.searchCTS.Token);
            });
            
        }

        private void RunSetGenListContent(CancellationToken ct)
        {
            try
            {
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    searchTerm.isIgnoringNextEvent = true;
                    if (!isShowingEventList)
                    {
                        isShowingEventList = true;
                        eventDisplayWrapper.Children.Remove(eventDisplay);

                        eventDisplayWrapper.Children.Add(genEventList);
                    }
                    loadingIndicator.IsVisible = true;
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


                    var itemTemplate = new DataTemplate(typeof(GenEventCell));
                    itemTemplate.CreateContent();

                    genEventListView.ItemTemplate = itemTemplate;

                    ListView listView = new ListView
                    {
                        //ItemsSource = GlobalVars.db.GetItems(),
                        ItemTemplate = itemTemplate
                    };

                    Task.Factory.StartNew(async () =>
                    {
                        try
                        {
                            var newItemsSource = await GlobalVars.db.GetItemsFTS4(searchEntry.Text, GetDBOptions());

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
                            genEventListView.ItemsSource = newItemsSource;
                                genEventList.Content = genEventListView;
                                loadingIndicator.IsVisible = false;
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

        private GenconMobileDatabase.DBOptions[] GetDBOptions()
        {
            List<GenconMobileDatabase.DBOptions> returnMe = new List<GenconMobileDatabase.DBOptions>();

            if (dayPicker.SelectedIndex != -1)
            {
                switch (dayPicker.Items[dayPicker.SelectedIndex])
                {
                    case "ALL":
                        returnMe.Add(GenconMobileDatabase.DBOptions.FilterAllDays);
                        break;
                    case "All Days":
                        returnMe.Add(GenconMobileDatabase.DBOptions.FilterAllDays);
                        break;
                    case "WED":
                        returnMe.Add(GenconMobileDatabase.DBOptions.FilterWednesday);
                        break;
                    case "THU":
                        returnMe.Add(GenconMobileDatabase.DBOptions.FilterThursday);
                        break;
                    case "FRI":
                        returnMe.Add(GenconMobileDatabase.DBOptions.FilterFriday);
                        break;
                    case "SAT":
                        returnMe.Add(GenconMobileDatabase.DBOptions.FilterSaturday);
                        break;
                    case "SUN":
                        returnMe.Add(GenconMobileDatabase.DBOptions.FilterSunday);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                returnMe.Add(GenconMobileDatabase.DBOptions.FilterAllDays);
            }
            

            if (afterPicker.SelectedIndex != -1)
            {
                switch (afterPicker.Items[afterPicker.SelectedIndex])
                {
                    case "---":
                        returnMe.Add(GenconMobileDatabase.DBOptions.After00);
                        break;
                    case "01:00AM":
                        returnMe.Add(GenconMobileDatabase.DBOptions.After01);
                        break;
                    case "02:00AM":
                        returnMe.Add(GenconMobileDatabase.DBOptions.After02);
                        break;
                    case "03:00AM":
                        returnMe.Add(GenconMobileDatabase.DBOptions.After03);
                        break;
                    case "04:00AM":
                        returnMe.Add(GenconMobileDatabase.DBOptions.After04);
                        break;
                    case "05:00AM":
                        returnMe.Add(GenconMobileDatabase.DBOptions.After05);
                        break;
                    case "06:00AM":
                        returnMe.Add(GenconMobileDatabase.DBOptions.After06);
                        break;
                    case "07:00AM":
                        returnMe.Add(GenconMobileDatabase.DBOptions.After07);
                        break;
                    case "08:00AM":
                        returnMe.Add(GenconMobileDatabase.DBOptions.After08);
                        break;
                    case "09:00AM":
                        returnMe.Add(GenconMobileDatabase.DBOptions.After09);
                        break;
                    case "10:00AM":
                        returnMe.Add(GenconMobileDatabase.DBOptions.After10);
                        break;
                    case "11:00AM":
                        returnMe.Add(GenconMobileDatabase.DBOptions.After11);
                        break;
                    case "12:00PM":
                        returnMe.Add(GenconMobileDatabase.DBOptions.After12);
                        break;
                    case "01:00PM":
                        returnMe.Add(GenconMobileDatabase.DBOptions.After13);
                        break;
                    case "02:00PM":
                        returnMe.Add(GenconMobileDatabase.DBOptions.After14);
                        break;
                    case "03:00PM":
                        returnMe.Add(GenconMobileDatabase.DBOptions.After15);
                        break;
                    case "04:00PM":
                        returnMe.Add(GenconMobileDatabase.DBOptions.After16);
                        break;
                    case "05:00PM":
                        returnMe.Add(GenconMobileDatabase.DBOptions.After17);
                        break;
                    case "06:00PM":
                        returnMe.Add(GenconMobileDatabase.DBOptions.After18);
                        break;
                    case "07:00PM":
                        returnMe.Add(GenconMobileDatabase.DBOptions.After19);
                        break;
                    case "08:00PM":
                        returnMe.Add(GenconMobileDatabase.DBOptions.After20);
                        break;
                    case "09:00PM":
                        returnMe.Add(GenconMobileDatabase.DBOptions.After21);
                        break;
                    case "10:00PM":
                        returnMe.Add(GenconMobileDatabase.DBOptions.After22);
                        break;
                    case "11:00PM":
                        returnMe.Add(GenconMobileDatabase.DBOptions.After23);
                        break;
                }
            }

            if (beforePicker.SelectedIndex != -1)
            {
                switch (beforePicker.Items[beforePicker.SelectedIndex])
                {
                    case "---":
                        returnMe.Add(GenconMobileDatabase.DBOptions.Before24);
                        break;
                    case "01:00AM":
                        returnMe.Add(GenconMobileDatabase.DBOptions.Before01);
                        break;
                    case "02:00AM":
                        returnMe.Add(GenconMobileDatabase.DBOptions.Before02);
                        break;
                    case "03:00AM":
                        returnMe.Add(GenconMobileDatabase.DBOptions.Before03);
                        break;
                    case "04:00AM":
                        returnMe.Add(GenconMobileDatabase.DBOptions.Before04);
                        break;
                    case "05:00AM":
                        returnMe.Add(GenconMobileDatabase.DBOptions.Before05);
                        break;
                    case "06:00AM":
                        returnMe.Add(GenconMobileDatabase.DBOptions.Before06);
                        break;
                    case "07:00AM":
                        returnMe.Add(GenconMobileDatabase.DBOptions.Before07);
                        break;
                    case "08:00AM":
                        returnMe.Add(GenconMobileDatabase.DBOptions.Before08);
                        break;
                    case "09:00AM":
                        returnMe.Add(GenconMobileDatabase.DBOptions.Before09);
                        break;
                    case "10:00AM":
                        returnMe.Add(GenconMobileDatabase.DBOptions.Before10);
                        break;
                    case "11:00AM":
                        returnMe.Add(GenconMobileDatabase.DBOptions.Before11);
                        break;
                    case "12:00PM":
                        returnMe.Add(GenconMobileDatabase.DBOptions.Before12);
                        break;
                    case "01:00PM":
                        returnMe.Add(GenconMobileDatabase.DBOptions.Before13);
                        break;
                    case "02:00PM":
                        returnMe.Add(GenconMobileDatabase.DBOptions.Before14);
                        break;
                    case "03:00PM":
                        returnMe.Add(GenconMobileDatabase.DBOptions.Before15);
                        break;
                    case "04:00PM":
                        returnMe.Add(GenconMobileDatabase.DBOptions.Before16);
                        break;
                    case "05:00PM":
                        returnMe.Add(GenconMobileDatabase.DBOptions.Before17);
                        break;
                    case "06:00PM":
                        returnMe.Add(GenconMobileDatabase.DBOptions.Before18);
                        break;
                    case "07:00PM":
                        returnMe.Add(GenconMobileDatabase.DBOptions.Before19);
                        break;
                    case "08:00PM":
                        returnMe.Add(GenconMobileDatabase.DBOptions.Before20);
                        break;
                    case "09:00PM":
                        returnMe.Add(GenconMobileDatabase.DBOptions.Before21);
                        break;
                    case "10:00PM":
                        returnMe.Add(GenconMobileDatabase.DBOptions.Before22);
                        break;
                    case "11:00PM":
                        returnMe.Add(GenconMobileDatabase.DBOptions.Before23);
                        break;
                }
            }


            string sortPickerChoice = "";
            if (sortPicker.SelectedIndex != -1)
            {
                sortPickerChoice = sortPicker.Items[sortPicker.SelectedIndex];
                switch (sortPicker.Items[sortPicker.SelectedIndex])
                {
                    case "Time":
                        returnMe.Add(GenconMobileDatabase.DBOptions.SortTime);
                        break;
                    case "Title":
                        returnMe.Add(GenconMobileDatabase.DBOptions.SortTitle);
                        break;
                    case "Ticket":
                        returnMe.Add(GenconMobileDatabase.DBOptions.SortTickets);
                        break;
                    case "Price":
                        returnMe.Add(GenconMobileDatabase.DBOptions.SortPrice);
                        break;
                    default:
                        returnMe.Add(GenconMobileDatabase.DBOptions.SortTime);
                        break;                            
                }
            }
            else
            {
                sortPickerChoice = "Time";
                returnMe.Add(GenconMobileDatabase.DBOptions.SortTime);
            }

            if (isSortDescending)
            {
                if (defaultSortChoices.First(d => d.Name == sortPickerChoice).isSortDescending)
                {
                    returnMe.Add(GenconMobileDatabase.DBOptions.SortDescending);
                }
                else
                {
                    returnMe.Add(GenconMobileDatabase.DBOptions.SortAscending);
                }
            }
            else
            {
                if (defaultSortChoices.First(d => d.Name == sortPickerChoice).isSortDescending)
                {
                    returnMe.Add(GenconMobileDatabase.DBOptions.SortAscending);
                }
                else
                {
                    returnMe.Add(GenconMobileDatabase.DBOptions.SortDescending);
                }
            }

            return returnMe.ToArray();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            GlobalVars.GenConBusiness.ShowLoadingEventMessage("Data is still loading, Events may not be up to date.");

            genEventListView?.ClearValue(ListView.SelectedItemProperty);


            //checkForNewEvents();
        }

        public void UpdateEventInfo()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                lastUpdatedEventsLabel.Text = GlobalVars.eventsLastUpdatedPretty;
                eventsTotalCountLabel.Text = GlobalVars.eventsTotalCountPretty;
            });
        }

        public async Task CloseAllPickers()
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                dayPicker.Unfocus();
                sortPicker.Unfocus();
                afterPicker.Unfocus();
                beforePicker.Unfocus();
            });
            await Task.Delay(250);
        }

        

    }
}
