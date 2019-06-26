    using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace Domotica.Models
{
    public class Profile
    {
        [PrimaryKey, AutoIncrement, Unique, NotNull]
        public int Id { get; set; }
        public int RFID { get; set; }
        public string AnimalName { get; set; }
        public int DefaultPortionSize { get; set; }
        public string AnimalType { get; set; }
    }
}