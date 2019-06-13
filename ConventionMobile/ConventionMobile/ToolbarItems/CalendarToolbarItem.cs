using ConventionMobile.Model;

namespace ConventionMobile.ToolbarItems
{
    public class CalendarToolbarItem : GenToolbarItem
    {
        public CalendarToolbarItem()
        {
            this.ImageSource = "ic_today_black_24dp.png";
            this.Title = "Add To Calendar";
            
            this.OnClickHandler += (sender, args) =>
            {
                var currentEvent = (GenEvent) this.BindingContext;
                GlobalVars.AddToCalendar(currentEvent);
            };
            this.Initialize();
        }
    }
}
