using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConventionMobile.iOS;
using Foundation;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(SafeAreaInsets_iOS))]

namespace ConventionMobile.iOS
{
    public class SafeAreaInsets_iOS : ISafeAreaInsets
    {
        public SafeAreaInsets_iOS()
        {
        }

        public Thickness Padding()
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
            {
                var sai = UIApplication.SharedApplication.KeyWindow.SafeAreaInsets;

                var returnMeFinal = new Thickness(sai.Left, sai.Top, sai.Right, sai.Bottom);

                //var horiz = returnMe.Left > returnMe.Right ? returnMe.Left : returnMe.Right;
                //var vert = returnMe.Top > returnMe.Bottom ? returnMe.Top : returnMe.Bottom;
                //return (int)(horiz > vert ? horiz : vert);
                return returnMeFinal;
            }
            else 
            {
                return new Thickness(0, 0, 0, 0);
            }
        }
    }
}