using ConventionMobile.Model;

namespace ConventionMobile.ToolbarItems
{
    public class CalendarToolbarItem : GenToolbarItem
    {
        private const string ImageSource = "ic_today_black_24dp.png";

        public CalendarToolbarItem()
        {
            this.Source = ImageSource;
            
            this.OnClickHandler += (sender, args) =>
            {
                var currentEvent = (GenEvent) this.BindingContext;
                GlobalVars.AddToCalendar(currentEvent);
            };
            this.AddGesture();
        }
    }
}
