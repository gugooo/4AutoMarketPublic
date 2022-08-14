using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;


namespace _4AutoMarket.Models
{
    public class GlobalSetings
    {
        public int Id { get; set; }
        public bool EnableAllReviews { get; set; }
        public string LiveChat_AutoResponse { get; set; }
        public string LiveChat_OperatorName { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal ShippingOrderSum { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal ShippingWhenMore { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal ShippingWhenLess { get; set; }
        public string IdramId { get; set; }
        public string IdramSecretKey { get; set; }
    }
}
