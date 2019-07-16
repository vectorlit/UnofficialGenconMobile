using ConventionMobile.Model;
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
    public class DisplayOptionsPage : PopupPage
    {
        Slider slider;
        StackLayout holderLayout;
        //StackLayout sliderTickLayout;
        Grid sliderCoverGrid;
        StackLayout buttonCoverLayout;
        Button cancelButton;
        Button okButton;

        List<KeyValuePair<int, DataTemplate>> genEventCellTemplateCache = new List<KeyValuePair<int, DataTemplate>>();

        ListView genEventDemoListView;

        public int originalValue = -1;

        private int oldSliderValue = -1;

        public DisplayOptionsPage()
        {
            originalValue = GlobalVars.fontSizeAdjustment;

            // Create outer holder
            holderLayout = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.White,
            };

            holderLayout.Children.Add(new Label { Text = "Change Font Size:", FontSize = 20 });

            sliderCoverGrid = new Grid
            {
                Padding = 0,
                WidthRequest = 351
            };

            //create slider and layout for slider to reside
            slider = new Slider
            {
                Minimum = GlobalVars.fontSizeAdjustmentMinimum,
                Maximum = GlobalVars.fontSizeAdjustmentMaximum,
                Value = GlobalVars.fontSizeAdjustment,
                WidthRequest = 350
            };

            slider.ValueChanged += (sender, e) =>
            {
                double StepValue = GlobalVars.fontSizeAdjustmentSteps;

                var newStep = Math.Round(e.NewValue / StepValue);

                int newValue = (int)(newStep * StepValue);

                slider.Value = newValue;

                if (oldSliderValue != newValue)
                {
                    updateFontSize(true);
                }

                oldSliderValue = newValue;
            };

            //sliderTickLayout = new StackLayout
            //{
            //    Orientation = StackOrientation.Horizontal,
            //    WidthRequest = 350
            //};

            //sliderCoverGrid.Children.Add(sliderTickLayout);
            sliderCoverGrid.Children.Add(slider);

            //sliderCoverLayout.Children.Add(slider);

            //AddTickMarksForSlider(sliderTickLayout);

            holderLayout.Children.Add(sliderCoverGrid);

            holderLayout.Children.Add(new Label { Text = "The font will look like this:", FontSize = 16 });

            var genEventCellTemplate = new DataTemplate(typeof(GenEventCell));
            genEventCellTemplate.CreateContent();
            genEventCellTemplateCache.Add(new KeyValuePair<int, DataTemplate>(GlobalVars.fontSizeAdjustment, genEventCellTemplate));

            genEventDemoListView = new ListView()
            {
                ItemTemplate = genEventCellTemplate,
                VerticalOptions = LayoutOptions.Start,
                ItemsSource = new List<GenEvent>() { new GenEvent() {
                        Title = "Super long event name - The Reckoning of The King Part IV",
                        Description = "Here lies a fantastic description, the most descriptive description ever to be described, of all descriptions beyond script",
                        Cost = "4",
                        AvailableTickets = 25,
                        StartDateTime = GlobalVars.startingDate,
                        EndDateTime = GlobalVars.startingDate.AddHours(2)
                    }
                },
                RowHeight = (int)GlobalVars.sizeListCellHeight,
                HeightRequest = 80
            };


            holderLayout.Children.Add(genEventDemoListView);
            

            // Add cancel and OK buttons
            buttonCoverLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal
            };

            cancelButton = new Button
            {
                Text = "Cancel"
            };

            cancelButton.Clicked += async (object sender, EventArgs e) =>
            {
                try
                {
                    GlobalVars.fontSizeAdjustment = originalValue;
                    await PopupNavigation.Instance.PopAsync();
                }
                catch (Exception)
                {

                }
            };

            okButton = new Button
            {
                Text = "OK"
            };


            okButton.Clicked += async (object sender, EventArgs e) =>
            {
                try
                {
                    updateFontSize();
                    GlobalVars.DoToast("Update success - **REFRESHING SCREEN**", GlobalVars.ToastType.Green);

                    GlobalVars.View_GenSearchView?.UpdateEventInfo();

                    if (GlobalVars.View_GenSearchView != null)
                    {
                        await GlobalVars.View_GenSearchView.CloseAllPickers();
                    }

                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        try
                        {
                            await Task.Delay(1000);
                            (Application.Current as App)?.ShowMainPage();
                        }
                        catch (Exception)
                        {

                        }
                    });

                    await PopupNavigation.Instance.PopAsync();
                }
                catch (Exception)
                {

                }
            };

            buttonCoverLayout.Children.Add(cancelButton);
            buttonCoverLayout.Children.Add(okButton);

            holderLayout.Children.Add(buttonCoverLayout);

            Content = holderLayout;
        }

        private void updateFontSize(bool updatePage = false)
        {
            GlobalVars.fontSizeAdjustment = (int)slider.Value;
            if (updatePage)
            {
                updateListView();
            }
        }

        private void updateListView()
        {
            var genEventCellTemplate = genEventCellTemplateCache.FirstOrDefault(d => d.Key == GlobalVars.fontSizeAdjustment);
            if (genEventCellTemplate.Equals(default(KeyValuePair<int, DataTemplate>)))
            {
                genEventCellTemplate = new KeyValuePair<int, DataTemplate>(GlobalVars.fontSizeAdjustment, new DataTemplate(typeof(GenEventCell)));
                genEventCellTemplate.Value.CreateContent();
                genEventCellTemplateCache.Add(genEventCellTemplate);
            }

            //genEventDemoListView = new ListView()
            //{
            //    ItemTemplate = genEventCellTemplate,
            //    VerticalOptions = LayoutOptions.Start,
            //    ItemsSource = new List<GenEvent>() { new GenEvent() {
            //            Title = "Super long event name - The Reckoning of The King Part IV",
            //            Description = "Here lies a fantastic description, the most descriptive description ever to be described, of all descriptions beyond script",
            //            Cost = "4",
            //            AvailableTickets = 25,
            //            StartDateTime = GlobalVars.startingDate,
            //            EndDateTime = GlobalVars.startingDate.AddHours(2)
            //        }
            //    },
            //    RowHeight = (int)GlobalVars.sizeListCellHeight
            //};

            genEventDemoListView.RowHeight = (int)GlobalVars.sizeListCellHeight;
            genEventDemoListView.ItemTemplate = genEventCellTemplate.Value;
        }


        // provides a visual indicator and was worth a shot but it doesn't work cross platform properly
        //public void AddTickMarksForSlider(StackLayout view)
        //{
        //    int ticksDivider = GlobalVars.fontSizeAdjustmentSteps;
        //    int ticks = (int)(slider.Maximum - slider.Minimum) / ticksDivider;

        //    view.BackgroundColor = Color.Transparent;

        //    // make a UIImageView with tick for each tick in the slider
        //    for (int i = 0; i <= ticks; i++)
        //    {

        //        Label tick = new Label();
        //        tick.WidthRequest = 2;
        //        tick.HeightRequest = 2;

        //        view.Padding = new Thickness(15, 0, 14, 0);

        //        tick.Margin = new Thickness((350 / ticks) * i, 0, 0, 0);

        //        tick.BackgroundColor = Color.Red;

        //        view.Children.Add(tick);

        //    }
        //}
    }
}
