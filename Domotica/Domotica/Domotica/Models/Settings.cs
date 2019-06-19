using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace Domotica.Models
{
    public class Settings
    {
        [PrimaryKey, AutoIncrement, Unique, NotNull]
        public int Settings_id { get; set; }
        public string Ip_config { get; set; }
    }
}