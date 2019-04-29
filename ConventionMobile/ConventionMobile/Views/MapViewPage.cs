using System;
using System.IO;
using System.Reflection;
using Xamarin.Forms;

namespace ConventionMobile.Views
{
    //public class BaseUrlWebView : WebView { }


    public class MapViewPage : ContentPage
    {
        CustomWebView customWeb;

        public MapViewPage()
        {
            Padding = new Thickness(0, 20, 0, 0);
            customWeb = new CustomWebView
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            Content = new StackLayout
            {
                Children = {
                        customWeb
                    }
            };

            
            //var browser = new BaseUrlWebView();

            //var htmlSource = new HtmlWebViewSource();

            //// do not set the BaseUrl on iOS because of the bug
            //if (Device.OS != TargetPlatform.iOS)
            //{
            //    // the BaseUrlWebViewRenderer does this for iOS, until bug is fixed
            //    htmlSource.BaseUrl = DependencyService.Get<IBaseURL>().Get();
            //}
            //Image boundImage = new Image { Aspect = Aspect.AspectFit };
            //boundImage.SetBinding(Image.SourceProperty, "dataImage");

            //Content = new Grid
            //{
            //    Padding = new Thickness(20),
            //    Children =
            //    {
            //        new PinchToZoomContainer
            //        {
            //          Content = boundImage
            //        }
            //    }
            //};

            //PROBLEM IS HERE
            //var data = ((DetailChoice)this.BindingContext).data;

            //if (data.ToLower().EndsWith(".html") || data.ToLower().EndsWith(".htm"))
            //{
            //    Content = new Grid
            //    {
            //        Padding = new Thickness(20),
            //        Children =
            //        {
            //            new PinchToZoomContainer
            //            {
            //              Content = new Image()
            //            }
            //        }
            //    };
            //}

            //Otherwise we pull in the image and encode to base64 and plunk it into the webview. stupid i know.
            //else
            //{

            //var data = "conventioncenter1.png";


            //var htmlSource = new HtmlWebViewSource();

            //var extension = Path.GetExtension(data).ToLower().Replace(".", "");
            //if (extension == "jpg")
            //{
            //    extension = "jpeg";
            //}

            //var assembly = typeof(GlobalVars).GetTypeInfo().Assembly;
            //Stream stream = assembly.GetManifestResourceStream("ConventionMobile.Resources." + data);

            //if (extension == "html" || extension == "htm")
            //{
            //    string text = "";
            //    using (var reader = new StreamReader(stream))
            //    {
            //        text = reader.ReadToEnd();
            //    }

            //    htmlSource.Html = text;   
            //}
            //else
            //{
            //    byte[] byteData = getByteData(stream);
            //    String encodedImageData = Convert.ToBase64String(byteData);

            //    string fart = String.Format("<html><head></head><body><img src=\"data:image/{0};base64,{1}\" /></body></html>", extension, encodedImageData);

            //    htmlSource.Html = fart;
            //}




            //htmlSource.Html = "fart";

            //htmlSource.SetBinding(HtmlWebViewSource.HtmlProperty, "formattedHTMLEmbeddedImageData");

            //}



            ////works but doesn't work
            //var htmlSource = new HtmlWebViewSource();
            //var browser = new WebView { Source = htmlSource };

            //Content = browser;


            //htmlSource.SetBinding(HtmlWebViewSource.HtmlProperty, "formattedHTMLEmbeddedImageData");
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            DetailChoice dc = (DetailChoice)this.BindingContext;

            if (dc != null)
            {
                var fileName = DependencyService.Get<IFileOps>().GetFileLocation(dc.data);

                if (DependencyService.Get<IFileOps>().FileExists(fileName))
                {
                    customWeb.Uri = fileName;
                }
                else
                {
                    customWeb.Uri = dc.data;
                }
            }
        }


        private byte[] getByteData(Stream sourceStream)
        {
            using (var memoryStream = new MemoryStream())
            {
                sourceStream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

    }


    public class PinchToZoomContainer : ContentView
    {
        private double currentScale = 1;
        private double startScale = 1;
        private double xOffset = 0;
        private double yOffset = 0;

        public PinchToZoomContainer()
        {
            var pinchGesture = new PinchGestureRecognizer();
            pinchGesture.PinchUpdated += OnPinchUpdated;
            GestureRecognizers.Add(pinchGesture);
        }

        public void OnPinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
        {
            if (e.Status == GestureStatus.Started)
            {
                // Store the current scale factor applied to the wrapped user interface element,
                // and zero the components for the center point of the translate transform.
                startScale = Content.Scale;
                Content.AnchorX = 0;
                Content.AnchorY = 0;
            }
            if (e.Status == GestureStatus.Running)
            {
                // Calculate the scale factor to be applied.
                currentScale += (e.Scale - 1) * startScale;
                currentScale = Math.Max(1, currentScale);

                // The ScaleOrigin is in relative coordinates to the wrapped user interface element,
                // so get the X pixel coordinate.
                double renderedX = Content.X + xOffset;
                double deltaX = renderedX / Width;
                double deltaWidth = Width / (Content.Width * startScale);
                double originX = (e.ScaleOrigin.X - deltaX) * deltaWidth;

                // The ScaleOrigin is in relative coordinates to the wrapped user interface element,
                // so get the Y pixel coordinate.
                double renderedY = Content.Y + yOffset;
                double deltaY = renderedY / Height;
                double deltaHeight = Height / (Content.Height * startScale);
                double originY = (e.ScaleOrigin.Y - deltaY) * deltaHeight;

                // Calculate the transformed element pixel coordinates.
                double targetX = xOffset - (originX * Content.Width) * (currentScale - startScale);
                double targetY = yOffset - (originY * Content.Height) * (currentScale - startScale);

                // Apply translation based on the change in origin.
                Content.TranslationX = Clamp(targetX, -Content.Width * (currentScale - 1), 0);
                Content.TranslationY = Clamp(targetY, -Content.Height * (currentScale - 1), 0);

                // Apply scale factor.
                Content.Scale = currentScale;
            }
            if (e.Status == GestureStatus.Completed)
            {
                // Store the translation delta's of the wrapped user interface element.
                xOffset = Content.TranslationX;
                yOffset = Content.TranslationY;
            }
        }

        public static T Clamp<T>(T value, T max, T min)
         where T : System.IComparable<T>
        {
            T result = value;
            if (value.CompareTo(max) > 0)
                result = max;
            if (value.CompareTo(min) < 0)
                result = min;
            return result;
        }
    }
}
