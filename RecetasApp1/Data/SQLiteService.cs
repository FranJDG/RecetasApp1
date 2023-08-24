using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecetasApp1.Data
{
    public class SQLiteService
    {
        public SQLiteConnection GetConnection()
        {
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "mydatabase.db3");
            return new SQLiteConnection(dbPath);
        }
    }
}
