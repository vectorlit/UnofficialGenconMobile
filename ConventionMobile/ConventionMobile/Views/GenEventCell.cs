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
    public class GenEventCell : ViewCell
    {
        private Label eventNameLabel;
        private Label priceLabel;
        private Label staticTicketNumbersPreface;
        private Label ticketNumbersLabel;
        private Label descriptionLabel;
        private Label timeLabel;
        private Grid wholeLayout;
       
        public GenEventCell()
        {
            //var longPressedEffect = Effect.Resolve("ConventionMobile.LongPressedEffect");
            //LongPressedEffect.SetCommand(this, new Command(() =>
            //{
            //    OnLongPressed(this, new LongPressEventArgs((GenEvent)this.BindingContext));
            //}));
            //this.Effects.Add(longPressedEffect);

            wholeLayout = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(5 - (GlobalVars.fontSizeAdjustment * .5), GridUnitType.Star) },
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
                FontSize = GlobalVars.sizeLarge + GlobalVars.fontSizeAdjustment,
                FontAttributes = FontAttributes.Bold,
                VerticalTextAlignment = TextAlignment.Center
                //HorizontalTextAlignment = TextAlignment.Start
            };
            //eventNameLabel.SetBinding(Label.TextProperty, "Title");
            wholeLayout.Children.Add(eventNameLabel, 0, 0);


            priceLabel = new Label
            {
                Text = "$-1",
                FontSize = GlobalVars.sizeLarge + GlobalVars.fontSizeAdjustment,
                LineBreakMode = LineBreakMode.NoWrap,
                TextColor = GlobalVars.colorMoney,
                HorizontalTextAlignment = TextAlignment.End,
                VerticalTextAlignment = TextAlignment.Center
            };
            //priceLabel.SetBinding(Label.TextProperty, "FormattedCost");
            wholeLayout.Children.Add(priceLabel, 1, 0);


            staticTicketNumbersPreface = new Label
            {
                Text = "Avail:",
                FontSize = GlobalVars.sizeMedium + GlobalVars.fontSizeAdjustment,
                LineBreakMode = LineBreakMode.NoWrap,
                VerticalTextAlignment = TextAlignment.Center
                //VerticalOptions = LayoutOptions.EndAndExpand

            };
            wholeLayout.Children.Add(staticTicketNumbersPreface, 2, 0);

            ticketNumbersLabel = new Label
            {
                Text = "-999",
                FontSize = GlobalVars.sizeLarge + GlobalVars.fontSizeAdjustment,
                LineBreakMode = LineBreakMode.NoWrap,
                HorizontalTextAlignment = TextAlignment.End,
                VerticalTextAlignment = TextAlignment.Center
            };
            //ticketNumbersLabel.SetBinding(Label.TextProperty, "AvailableTickets");
            //ticketNumbersLabel.SetBinding(Label.TextColorProperty, new Binding("AvailableTickets", converter: new AvailableAmountConverter()));
            wholeLayout.Children.Add(ticketNumbersLabel, 3, 0);

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
                FontSize = GlobalVars.sizeSmall + GlobalVars.fontSizeAdjustment,
                LineBreakMode = LineBreakMode.TailTruncation,
                VerticalTextAlignment = TextAlignment.Center
            };
            //descriptionLabel.SetBinding(Label.TextProperty, "Description");
            wholeLayout.Children.Add(descriptionLabel, 0, 1);

            //secondLine.Children.Add(descriptionLabel);


            timeLabel = new Label
            {
                Text = "Wed@10:00(2hrs)",
                FontSize = GlobalVars.sizeSmall + GlobalVars.fontSizeAdjustment,
                HorizontalTextAlignment = TextAlignment.End,
                VerticalTextAlignment = TextAlignment.Center
                //MinimumWidthRequest = MinWidthSize
            };
            //timeLabel.SetBinding(Label.TextProperty, "FormattedDate");
            wholeLayout.Children.Add(timeLabel, 1, 1);
            Grid.SetColumnSpan(timeLabel, 3);

            //secondLine.Children.Add(timeLabel);


            //wholeLayout.Children.Add(firstLine);
            //wholeLayout.Children.Add(secondLine);

            //wholeLayout.SetBinding(Layout.BackgroundColorProperty, new Binding("BackgroundColor"));

            //SetupCell(true);

            View = wholeLayout;
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            var eventItem = BindingContext as GenEvent;
            if (eventItem != null)
            {
                eventNameLabel.Text = eventItem.Title;
                priceLabel.Text = eventItem.FormattedCost;
                ticketNumbersLabel.Text = eventItem.AvailableTickets.ToString();
                ticketNumbersLabel.TextColor = eventItem.AvailableTickets > 0 ? GlobalVars.colorTicketsAvailable : GlobalVars.colorTicketsUnavailable;
                descriptionLabel.Text = eventItem.Description;
                timeLabel.Text = eventItem.FormattedDate;
                wholeLayout.BackgroundColor = eventItem.BackgroundColor;
                //wholeLayout.SetBinding(Layout.BackgroundColorProperty, new Binding("BackgroundColor"));
            }
        }


        private void SetupCell(bool isRecycled)
        {
            //eventNameLabel.SetBinding(Label.TextProperty, "Title");
            //priceLabel.SetBinding(Label.TextProperty, "FormattedCost");
            //ticketNumbersLabel.SetBinding(Label.TextProperty, "AvailableTickets");
            //ticketNumbersLabel.SetBinding(Label.TextColorProperty, new Binding("AvailableTickets", converter: new AvailableAmountConverter()));
            //descriptionLabel.SetBinding(Label.TextProperty, "Description");
            //timeLabel.SetBinding(Label.TextProperty, "FormattedDate");
            //wholeLayout.SetBinding(Layout.BackgroundColorProperty, new Binding("BackgroundColor"));


            var eventItem = BindingContext as GenEvent;
            if (eventItem != null)
            {
                eventNameLabel.Text = eventItem.Title;
                priceLabel.Text = eventItem.FormattedCost;
                ticketNumbersLabel.Text = eventItem.AvailableTickets.ToString();
                ticketNumbersLabel.TextColor = eventItem.AvailableTickets > 0 ? GlobalVars.colorTicketsAvailable : GlobalVars.colorTicketsUnavailable;
                descriptionLabel.Text = eventItem.Description;
                timeLabel.Text = eventItem.FormattedDate;
                //wholeLayout.SetBinding(Layout.BackgroundColorProperty, new Binding("BackgroundColor"));
            }
        }

        //public delegate void GenEventLongPressEventHandler(object sender, GenEventLongPressArgs e);

        //public class GenEventLongPressArgs : EventArgs
        //{
        //    public GenEvent genEvent { get; private set; }
        //    public GenEventLongPressArgs(GenEvent genEvent)
        //    {
        //        this.genEvent = genEvent;
        //    }
        //}

        ///// <summary>
        ///// Assistance converter for text color binding on the available amount of tickets. Returns one color for zero tickets, a different color for any other positive amount.
        ///// </summary>
        //private class AvailableAmountConverter : IValueConverter
        //{
        //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        //    {
        //        var intValue = (int)value;

        //        if (intValue > 0)
        //        {
        //            return GlobalVars.colorTicketsAvailable;
        //        }

        //        return GlobalVars.colorTicketsUnavailable;
        //    }

        //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        //    {
        //        throw new NotImplementedException("Not implemented.");
        //    }
        //}

        //private class AvailableAmountBackgroundConverter : IValueConverter
        //{
        //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        //    {
        //        var intValue = (int)value;

        //        if (intValue > 0)
        //        {
        //            return GlobalVars.bgColorTicketsAvailable;
        //        }

        //        return GlobalVars.bgColorTicketsUnavailable;
        //    }

        //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        //    {
        //        throw new NotImplementedException("Not implemented.");
        //    }
        //}
    }
}
