using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _4AutoMarket.Models
{
    public class SubscribeEmail
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public bool isNew { get; set; }
        public DateTime Created { get; set; }
    }
}
