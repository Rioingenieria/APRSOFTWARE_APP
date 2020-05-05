using System;
using System.Collections.Generic;
using System.Text;
using SQLite;


namespace APRSOFTWARE_APP 
{ 
    public interface ISqlite
    {
        SQLiteAsyncConnection GetConnection();
    }
}
