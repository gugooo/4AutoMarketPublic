using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _4AutoMarket.Models
{
    public class MailingViewModel
    {
        public bool RegistredUsers { get; set; }
        public bool SubscribedUsers { get; set; }
        public bool NewSubscribedUsers { get; set; }
        public string OtherUsers { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
    }
}
