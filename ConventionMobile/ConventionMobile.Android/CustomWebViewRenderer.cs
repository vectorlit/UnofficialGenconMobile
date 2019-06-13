using Android.Content;
using ConventionMobile.Droid;
using ConventionMobile.Views;
using System.Net;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomWebView), typeof(CustomWebViewRenderer))]
namespace ConventionMobile.Droid
{
    public class CustomWebViewRenderer : WebViewRenderer
    {
        public CustomWebViewRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<WebView> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                var customWebView = Element as CustomWebView;
                Control.Settings.AllowUniversalAccessFromFileURLs = true;
                Control.Settings.BuiltInZoomControls = true;
                Control.Settings.DisplayZoomControls = true;

                if (customWebView.Uri.StartsWith("/"))
                {
                    //Control.LoadUrl(string.Format("file://{0}", customWebView.Uri));
                    if (isImageExtension(customWebView.Uri))
                    {
                        Control.LoadDataWithBaseURL(null, "<style>img{display: inline;height: auto;max-width: 100%;}</style><img src=\"" + string.Format("file://{0}", customWebView.Uri) + "\" />", "text/html", "UTF-8", null);
                    }
                    else
                    {
                        Control.LoadUrl(string.Format("file://{0}", customWebView.Uri));
                    }
                }
                else
                {
                    if (isImageExtension(customWebView.Uri))
                    {
                        Control.LoadDataWithBaseURL(null, "<style>img{display: inline;height: auto;max-width: 100%;}</style><img src=\"" + string.Format("file:///android_asset/{0}", WebUtility.UrlEncode(customWebView.Uri)) + "\" />", "text/html", "UTF-8", null);
                    }
                    else
                    {
                        if (customWebView.Uri.ToLower().StartsWith("http"))
                        {
                            Control.LoadUrl(customWebView.Uri);
                        }
                        else
                        {
                            Control.LoadUrl(string.Format("file:///android_asset/{0}", WebUtility.UrlEncode(customWebView.Uri)));
                        }
                        
                    }

                }                
            }
        }

        private bool isImageExtension(string uri)
        {
            var test = uri.ToLower();

            return test.EndsWith("jpg")
                || test.EndsWith("jpeg")
                || test.EndsWith("bmp")
                || test.EndsWith("gif")
                || test.EndsWith("png")
                || test.EndsWith("svg");
        }
    }
}