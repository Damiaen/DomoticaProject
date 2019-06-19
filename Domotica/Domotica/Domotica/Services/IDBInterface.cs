using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace Domotica.Services
{
    public interface IDBInterface
    {
        SQLiteConnection CreateConnection();
    }
}
