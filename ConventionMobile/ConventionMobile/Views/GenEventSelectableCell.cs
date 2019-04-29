using ConventionMobile.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

using Xamarin.Forms;

namespace ConventionMobile.Views
{
    public class GenEventSelectableCell : ViewCell
    {
        private Label eventNameLabel;
        private Label priceLabel;
        private Label staticTicketNumbersPreface;
        private Label ticketNumbersLabel;
        private Label descriptionLabel;
        private Label timeLabel;
        private Grid dataLayout;
        private StackLayout cellHolder;
        private BoxView checkBox;
        
        public GenEventSelectableCell()
        {
            cellHolder = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Padding = 0,
                Margin = 0,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };

            checkBox = new BoxView
            {
                IsVisible = false,
                WidthRequest = 12,
                HeightRequest = 12,
                Color = Color.DarkRed
            };

            dataLayout = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(4, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                },
                RowDefinitions =
                {
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }
                },
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.Fill,
                Padding = new Thickness(6, 0, 6, 0)
            };

            //var MinWidthSize = GenListSize.Width / 3;

            //StackLayout wholeLayout = new StackLayout { Orientation = StackOrientation.Vertical, HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand, Padding = 2, Spacing = 0 };

            //StackLayout firstLine = new StackLayout { Orientation = StackOrientation.Horizontal, Padding = new Thickness(0, 0), Spacing = 0 };

            //StackLayout moneyAndAvailableLayout = new StackLayout { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.EndAndExpand, Padding = 0, Spacing = 6, IsClippedToBounds = false, MinimumWidthRequest = MinWidthSize };

            eventNameLabel = new Label
            {
                Text = "Event Name Goes Here",
                LineBreakMode = LineBreakMode.TailTruncation,
                FontSize = GlobalVars.sizeLarge,
                FontAttributes = FontAttributes.Bold,
                //HorizontalTextAlignment = TextAlignment.Start
            };
            //eventNameLabel.SetBinding(Label.TextProperty, "Title");
            dataLayout.Children.Add(eventNameLabel, 0, 0);


            priceLabel = new Label
            {
                Text = "$-1",
                FontSize = GlobalVars.sizeLarge,
                LineBreakMode = LineBreakMode.NoWrap,
                TextColor = GlobalVars.colorMoney,
                HorizontalTextAlignment = TextAlignment.End
            };
            //priceLabel.SetBinding(Label.TextProperty, "FormattedCost");
            dataLayout.Children.Add(priceLabel, 1, 0);


            staticTicketNumbersPreface = new Label
            {
                Text = "Avail:",
                FontSize = GlobalVars.sizeMedium,
                LineBreakMode = LineBreakMode.NoWrap,
                VerticalTextAlignment = TextAlignment.End
                //VerticalOptions = LayoutOptions.EndAndExpand

            };
            dataLayout.Children.Add(staticTicketNumbersPreface, 2, 0);

            ticketNumbersLabel = new Label
            {
                Text = "-999",
                FontSize = GlobalVars.sizeLarge,
                LineBreakMode = LineBreakMode.NoWrap,
                HorizontalTextAlignment = TextAlignment.End
            };
            //ticketNumbersLabel.SetBinding(Label.TextProperty, "AvailableTickets");
            //ticketNumbersLabel.SetBinding(Label.TextColorProperty, new Binding("AvailableTickets", converter: new AvailableAmountConverter()));
            dataLayout.Children.Add(ticketNumbersLabel, 3, 0);

            //wholeLayout.SetBinding(Layout.BackgroundColorProperty, new Binding("AvailableTickets", converter: new AvailableAmountBackgroundConverter()));



            //moneyAndAvailableLayout.Children.Add(priceLabel);
            //moneyAndAvailableLayout.Children.Add(staticTicketNumbersPreface);
            //moneyAndAvailableLayout.Children.Add(ticketNumbersLabel);

            //firstLine.Children.Add(eventNameLabel);
            //firstLine.Children.Add(moneyAndAvailableLayout);

            //StackLayout secondLine = new StackLayout { Orientation = StackOrientation.Horizontal, Padding = new Thickness(0, 0), Spacing = 0 };

            descriptionLabel = new Label
            {
                Text = "Event Game, Event Game Edition, Etc Information",
                FontSize = GlobalVars.sizeSmall,
                LineBreakMode = LineBreakMode.TailTruncation
            };
            //descriptionLabel.SetBinding(Label.TextProperty, "Description");
            dataLayout.Children.Add(descriptionLabel, 0, 1);

            //secondLine.Children.Add(descriptionLabel);


            timeLabel = new Label
            {
                Text = "Wed@10:00(2hrs)",
                FontSize = GlobalVars.sizeSmall,
                HorizontalTextAlignment = TextAlignment.End,
                //MinimumWidthRequest = MinWidthSize
            };
            //timeLabel.SetBinding(Label.TextProperty, "FormattedDate");
            dataLayout.Children.Add(timeLabel, 1, 1);
            Grid.SetColumnSpan(timeLabel, 3);

            //secondLine.Children.Add(timeLabel);


            //wholeLayout.Children.Add(firstLine);
            //wholeLayout.Children.Add(secondLine);

            //wholeLayout.SetBinding(Layout.BackgroundColorProperty, new Binding("BackgroundColor"));

            //SetupCell(true);

            cellHolder.Children.Add(checkBox);

            cellHolder.Children.Add(dataLayout);

            var longPressedEffect = Effect.Resolve("ConventionMobile.LongPressedEffect");
            LongPressedEffect.SetCommand(cellHolder, new Command((object args) =>
            {
                if (BindingContext != null)
                {
                    var currentEvent = (GenEvent)BindingContext;
                    var tapType = args as LongPressedEffect.EffectType?;
                    if (tapType != null && (LongPressedEffect.EffectType)tapType == LongPressedEffect.EffectType.LongPress)
                    {
                        currentEvent.IsLongPressSelected = !currentEvent.IsLongPressSelected;
                    }
                    else
                    {
                        currentEvent.IsTapped = true;
                    }
                    
                    //currentEvent.IsSelected &= currentEvent.IsLongPressSelected;
                }
            }));

            cellHolder.Effects.Add(longPressedEffect);

            View = cellHolder;
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            //var longPressedEffect = Effect.Resolve("ConventionMobile.LongPressedEffect");
            ////LongPressedEffect.SetCommand(this, new Command(() =>
            ////{
            ////    OnLongPressed(this, new GenEventLongPressArgs((GenEvent)this.BindingContext));
            ////}));
            //this.Effects.Add(longPressedEffect);
            ////this.SetBinding()

            //this.Effects.Add(new LongPressedEffect());

            var eventItem = BindingContext as GenEvent;
            if (eventItem != null)
            {
                eventNameLabel.Text = eventItem.Title;
                priceLabel.Text = eventItem.FormattedCost;
                ticketNumbersLabel.Text = eventItem.AvailableTickets.ToString();
                ticketNumbersLabel.TextColor = eventItem.AvailableTickets > 0 ? GlobalVars.colorTicketsAvailable : GlobalVars.colorTicketsUnavailable;
                descriptionLabel.Text = eventItem.Description;
                timeLabel.Text = eventItem.FormattedDate;
                eventItem.BackgroundColor = GlobalVars.bgColorTicketsAvailable;
                dataLayout.BackgroundColor = eventItem.BackgroundColor;

                dataLayout.SetBinding(Layout.BackgroundColorProperty, new Binding("BackgroundColor"));
                checkBox.SetBinding(Layout.IsVisibleProperty, new Binding("IsSelected"));
            }
        }


        private void SetupCell(bool isRecycled)
        {
            var eventItem = BindingContext as GenEvent;

            //var longPressedEffect = Effect.Resolve("ConventionMobile.LongPressedEffect");
            ////LongPressedEffect.SetCommand(this, new Command(() =>
            ////{
            ////    OnLongPressed(this, new GenEventLongPressArgs((GenEvent)this.BindingContext));
            ////}));
            //this.Effects.Add(longPressedEffect);
            ////this.SetBinding()

            //this.Effects.Add(new LongPressedEffect());

            if (eventItem != null)
            {
                eventNameLabel.Text = eventItem.Title;
                priceLabel.Text = eventItem.FormattedCost;
                ticketNumbersLabel.Text = eventItem.AvailableTickets.ToString();
                ticketNumbersLabel.TextColor = eventItem.AvailableTickets > 0 ? GlobalVars.colorTicketsAvailable : GlobalVars.colorTicketsUnavailable;
                descriptionLabel.Text = eventItem.Description;
                timeLabel.Text = eventItem.FormattedDate;
                eventItem.BackgroundColor = GlobalVars.bgColorTicketsAvailable;
                dataLayout.SetBinding(Layout.BackgroundColorProperty, new Binding("BackgroundColor"));
                checkBox.SetBinding(Layout.IsVisibleProperty, new Binding("IsSelected"));
            }
        }

        public delegate void GenEventLongPressEventHandler(object sender, GenEventLongPressArgs e);

        public class GenEventLongPressArgs : EventArgs
        {
            public GenEvent genEvent { get; private set; }
            public GenEventLongPressArgs(GenEvent genEvent)
            {
                this.genEvent = genEvent;
            }
        }
    }
}
