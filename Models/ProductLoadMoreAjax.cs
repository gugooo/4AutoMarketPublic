using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _4AutoMarket.Models
{
    public class ProductLoadMoreAjax
    {
        public string id { get; set; }
        public int index { get; set; }
        public string brand { get; set; }
        public string serchKeys { get; set; }
        public IEnumerable<string> atributeIds { get; set; }
    }
}
