using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _4AutoMarket.Models
{
    public class MailingHistory
    {
        public int Id { get; set; }
        public string to { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public int MessageCount { get; set; }
        public DateTime Created { get; set; }
    }
}
