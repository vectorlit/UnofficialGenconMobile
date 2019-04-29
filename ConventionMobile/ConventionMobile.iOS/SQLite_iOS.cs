using System;
using ConventionMobile;
using Xamarin.Forms;
using ConventionMobile.iOS;
using System.IO;

[assembly: Dependency(typeof(SQLite_iOS))]

namespace ConventionMobile.iOS
{
    public class SQLite_iOS : ISQLite
    {
        public SQLite_iOS()
        {
        }

        #region ISQLite implementation
        public SQLite.SQLiteAsyncConnection GetConnection()
        {
            var oldSqliteFilename = "GenconMobileSQLite.db3";
            var sqliteFilename = "GenconMobile4SQLite.db3";
            //string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documents folder
            //string libraryPath = Path.Combine(documentsPath, "..", "Library"); // Library folder
            //var path = Path.Combine(libraryPath, sqliteFilename);
            var libraryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "..", "Library");
            var cachePath = Path.Combine(libraryPath, "Caches");
            var finalPath = Path.Combine(cachePath, sqliteFilename);
            var oldFinalPath = Path.Combine(cachePath, oldSqliteFilename);

            if (File.Exists(oldFinalPath))
            {
                File.Delete(oldFinalPath);
            }

            // This is where we copy in the prepopulated database
            //Console.WriteLine(path);
            //if (!File.Exists(path))
            //{
            //    File.Copy(sqliteFilename, path);
            //}

            var conn = new SQLite.SQLiteAsyncConnection(finalPath);

            // Return the database connection 
            return conn;
        }

        //public SQLite.SQLiteAsyncConnection DropAndRecreateThenGetConnection()
        //{
        //    var sqliteFilename = "GenconMobileSQLite.db3";
        //    //string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documents folder
        //    //string libraryPath = Path.Combine(documentsPath, "..", "Library"); // Library folder
        //    //var path = Path.Combine(libraryPath, sqliteFilename);
        //    var libraryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "..", "Library");
        //    var cachePath = Path.Combine(libraryPath, "Caches");
        //    var finalPath = Path.Combine(cachePath, sqliteFilename);
            
        //    if (File.Exists(finalPath))
        //    {
        //        File.Delete(finalPath);
        //    }

        //    var conn = new SQLite.SQLiteAsyncConnection(finalPath);

        //    // Return the database connection 
        //    return conn;
        //}
        #endregion
    }
}
