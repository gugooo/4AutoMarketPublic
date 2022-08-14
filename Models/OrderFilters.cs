using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _4AutoMarket.Models
{
    public class OrderFilters
    {
        public DateTime from { get; set; }
        public DateTime to { get; set; }
        public string orderNumber { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public Order.StatusType status { get; set; }
    }
}
