using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _4AutoMarket.Models
{
    public class RequestEmail
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public bool isNew { get; set; }
        public DateTime Created { get; set; }
    }
}
