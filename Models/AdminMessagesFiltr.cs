using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _4AutoMarket.Models
{
    public class AdminMessagesFiltr
    {
        public DateTime from { get; set; }
        public DateTime to { get; set; }
        public bool OnlyNew { get; set; }
        public string Email { get; set; }
    }
}
