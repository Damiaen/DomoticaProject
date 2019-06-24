using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace Domotica.Models
{
    public class Schedule
    {
        [PrimaryKey, AutoIncrement, Unique, NotNull]
        public int Schedule_id { get; set; }
        public int Profile_id { get; set; }
        public string ScheduleInfo { get; set; }
    }
}