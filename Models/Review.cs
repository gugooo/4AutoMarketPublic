using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _4AutoMarket.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string Description { get; set; }
        public bool? Status { get; set; }
        public DateTime Date { get; set; }
        public virtual User FK_User { get; set; }
        public virtual Product FK_Product { get; set; }
    }
}
