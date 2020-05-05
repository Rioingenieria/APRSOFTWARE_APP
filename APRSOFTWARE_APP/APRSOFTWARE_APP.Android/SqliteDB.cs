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
using Xamarin.Forms;
using APRSOFTWARE_APP.Droid;
using SQLite;
using System.IO;

[assembly: Dependency(typeof(SqliteDB))]
namespace APRSOFTWARE_APP.Droid
{
   public class SqliteDB : ISqlite
    {
        public SQLiteAsyncConnection GetConnection()
        {
            var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            var path = Path.Combine(documentsPath, "APPRSoftware.db3");
            return new SQLiteAsyncConnection(path);
        }
    }
}