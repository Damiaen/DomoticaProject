using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace Domotica.Models
{
    public class Profile
    {
        [PrimaryKey, AutoIncrement, Unique, NotNull]
        public int Profile_id { get; set; }
        public int RFID_id { get; set; }
        public string AnimalName { get; set; }
        public string PortionSize { get; set; }
    }
}