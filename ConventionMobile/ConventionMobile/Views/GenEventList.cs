using Xamarin.Forms;

namespace ConventionMobile.Views
{
    public class GenEventList : ContentView
    {
        ListView listView;

        //public double CalcWidth { get; set; } //Width

        //public double CalcHeight { get; set; } //Height

        public GenEventList()
        {
            DataTemplate itemTemplate = new DataTemplate(typeof(GenEventCell));
            
            listView = new ListView(ListViewCachingStrategy.RetainElement);
            listView.ItemTemplate = itemTemplate;
            
            Content = listView;

            //listView.OnLongPressed += List_LongPressed;
        }

        //private void List_LongPressed(object sender, LongPressEventArgs e)
        //{
        //    OnLongPressed?.Invoke(sender, e);
        //}

        //public async Task LoadContentTest()
        //{
        //    var itemTemplate = new DataTemplate(typeof(GenEventCell));
        //    itemTemplate.CreateContent();

        //    listView = new ListView
        //    {
        //        ItemsSource = await App.GenEventManager.Test100(),
        //        ItemTemplate = itemTemplate
        //    };

        //    this.Content = listView;
        //    //listView.ItemsSource = await App.GenEventManager.Test100();
        //}

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            
            //listView.ItemsSource = await App.GenEventManager.Test100();

        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            //GenListSize.Width = width;
            //GenListSize.Height = height;
        }
        
    }

    //public static class GenListSize
    //{

    //    public static double Width { get; set; } //Width

    //    public static double Height { get; set; } //Height
    //}
}
