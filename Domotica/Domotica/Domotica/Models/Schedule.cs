using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace Domotica.Models
{
    public class Schedule
    {
        [PrimaryKey, AutoIncrement, Unique, NotNull]
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public string FeedDate { get; set; }
        public string FeedTime { get; set; }
        public int PortionSize { get; set; }
        public string Description { get; set; }
        public int Expired { get; set; }
    }
}