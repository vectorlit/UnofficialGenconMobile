using System;
using ConventionMobile;
using Xamarin.Forms;
using ConventionMobile.Droid;
using System.IO;
using SQLite;
using System.Threading.Tasks;

[assembly: Dependency(typeof(SQLite_Android))]
namespace ConventionMobile.Droid
{
    public class SQLite_Android : ISQLite
    {
        public SQLite_Android()
        {
        }

        #region ISQLite implementation
        public SQLiteAsyncConnection GetConnection()
        {
            SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_e_sqlite3());
            //SQLitePCL.Batteries_V2.Init();
            var sqliteFilename = "GenconMobile4SQLite.db3";
            var oldSqliteFilename = "GenconMobileSQLite.db3";
            string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal); // Documents folder
            var path = Path.Combine(documentsPath, sqliteFilename);
            var oldPath = Path.Combine(documentsPath, oldSqliteFilename);

            if (File.Exists(oldPath))
            {
                File.Delete(oldPath);
            }

            SQLiteAsyncConnection returnMe;

            //if (!File.Exists(path))
            //{
            //    //var conn2 = new SQLiteAsyncConnection(path);
            //    //conn2.
            //    File.Create(path);
            //    returnMe = new SQLiteAsyncConnection(path, SQLiteOpenFlags.Create);
            //}
            //else
            //{
                returnMe = new SQLiteAsyncConnection(path);
            //}
            
            // This is where we copy in the prepopulated database
            //Console.WriteLine(path);
            //if (!File.Exists(path))
            //{
            //    var s = Forms.Context.Resources.OpenRawResource(Resource.Raw.GenconMobileSQLite);  // RESOURCE NAME ###
            
            //    // create a write stream
            //    FileStream writeStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
            //    // write to the stream
            //    ReadWriteStream(s, writeStream);
            //}

            // Return the database connection 
            return returnMe;
        }

        //public SQLiteAsyncConnection DropAndRecreateThenGetConnection()
        //{
        //    var sqliteFilename = "GenconMobileSQLite.db3";
        //    string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal); // Documents folder
        //    var path = Path.Combine(documentsPath, sqliteFilename);

        //    if (File.Exists(path))
        //    {
        //        File.Delete(path);
        //        File.Create(path);
        //    }

        //    var conn = new SQLiteAsyncConnection(path);

        //    // Return the database connection 
        //    return conn;
        //}
        #endregion

        ///// <summary>
        ///// helper method to get the database out of /raw/ and into the user filesystem
        ///// </summary>
        //void ReadWriteStream(Stream readStream, Stream writeStream)
        //{
        //    int Length = 256;
        //    Byte[] buffer = new Byte[Length];
        //    int bytesRead = readStream.Read(buffer, 0, Length);
        //    // write the required bytes
        //    while (bytesRead > 0)
        //    {
        //        writeStream.Write(buffer, 0, bytesRead);
        //        bytesRead = readStream.Read(buffer, 0, Length);
        //    }
        //    readStream.Close();
        //    writeStream.Close();
        //}
    }
}
