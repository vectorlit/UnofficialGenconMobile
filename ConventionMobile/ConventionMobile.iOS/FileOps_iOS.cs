using ConventionMobile.iOS;
using System.IO;
using Xamarin.Forms;
using System.Net;
using System;

[assembly: Dependency(typeof(FileOps_iOS))]
namespace ConventionMobile.iOS
{
    public class FileOps_iOS : IFileOps
    {
        public FileOps_iOS()
        {

        }

        bool IFileOps.FileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        string IFileOps.GetFileLocation(string fileName)
        {
            //string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documents folder
            //string libraryPath = Path.Combine(documentsPath, "..", "Library"); // Library folder
            var libraryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "..", "Library");
            var cachePath = Path.Combine(libraryPath, "Caches");
            var finalPath = Path.Combine(cachePath, fileName);
            //var path = Path.Combine(libraryPath, fileName);
            return finalPath;
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