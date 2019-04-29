using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConventionMobile.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(SafeAreaInsets_Android))]

namespace ConventionMobile.Droid
{
    public class SafeAreaInsets_Android : ISafeAreaInsets
    {
        public SafeAreaInsets_Android()
        {
        }

        public Thickness Padding()
        {
            return new Thickness(0, 0, 0, 0);
        }
    }
}