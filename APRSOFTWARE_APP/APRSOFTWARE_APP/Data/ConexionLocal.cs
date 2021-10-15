using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace APRSOFTWARE_APP.Data
{
   public  class ConexionLocal
    {
        public static SQLiteConnection cnSqlite = new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "APPRSoftware.db3"));

    }
}
