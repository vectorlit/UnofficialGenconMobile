using Foundation;
using ConventionMobile.iOS;
using ConventionMobile.Views;
using System.IO;
using System.Net;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomWebView), typeof(CustomWebViewRenderer))]
namespace ConventionMobile.iOS
{
    public class CustomWebViewRenderer : ViewRenderer<CustomWebView, UIWebView>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<CustomWebView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                SetNativeControl(new UIWebView());
            }
            if (e.OldElement != null)
            {
                // Cleanup
            }
            if (e.NewElement != null)
            {
                var customWebView = Element as CustomWebView;

                if (customWebView.Uri.StartsWith("/"))
                {
                    string fileName = Path.Combine(NSBundle.MainBundle.BundlePath, string.Format("{0}", customWebView.Uri));
                    Control.LoadRequest(new NSUrlRequest(new NSUrl(fileName, false)));
                    Control.ScalesPageToFit = true;
                }
                else
                {
                    string fileName = "";
                    if (customWebView.Uri.ToLower().StartsWith("http"))
                    {
                        fileName = customWebView.Uri;
                    }
                    else
                    {
                        string uri = customWebView.Uri;
                        fileName = Path.Combine(NSBundle.MainBundle.BundlePath, string.Format("Content/{0}", WebUtility.UrlEncode(uri)));
                    }
                    Control.LoadRequest(new NSUrlRequest(new NSUrl(fileName, false)));
                    Control.ScalesPageToFit = true;
                }

                
            }
        }
    }
}