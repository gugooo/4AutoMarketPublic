using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _4AutoMarket.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public bool IsNew { get; set; }
        public DateTime SendedDate { get; set; }
        public virtual User FK_UserSend { get; set; }
        public virtual User FK_AdminSend { get; set; }
    }

    public class MessageWrapper
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public bool IsNew { get; set; }
        public bool IsShare { get; set; }
        public DateTime Date { get; set; }

    }
}
