using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace _4AutoMarket.Models
{
    public class PromoCode
    {
        public int id { get; set; }
        public string Name { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal SalePercent { get; set; }
        public bool isActive { get; set; }
        public DateTime Created { get; set; }
        public DateTime Expired { get; set; }
    }
}
