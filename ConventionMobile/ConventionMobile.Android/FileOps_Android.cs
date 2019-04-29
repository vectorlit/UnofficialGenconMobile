using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ConventionMobile.Droid;
using System.IO;
using Xamarin.Forms;
using System.Net;

[assembly: Dependency(typeof(FileOps_Android))]
namespace ConventionMobile.Droid
{

    public class FileOps_Android : IFileOps
    {
        public FileOps_Android()
        {

        }

        bool IFileOps.FileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        string IFileOps.GetFileLocation(string fileName)
        {
            string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal); // Documents folder
            var path = Path.Combine(documentsPath, fileName);
            return path;
        }

        bool IFileOps.SaveFile(byte[] bytes, string fileName)
        {
            try
            {
                File.WriteAllBytes(fileName, bytes);
            }
            catch (Exception ex)
            {
                string blah = ex.Message;
                return false;
            }
            return true;
        }
    }
}