using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _4AutoMarket.Models
{
    public class SharedMessageUser
    {
        public int Id { get; set; }
        public bool IsNew { get; set; }
        public virtual SharedMessage Message { get; set; }
    }

    public class SharedMessage
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public DateTime SendedDate { get; set; }
    }
}
