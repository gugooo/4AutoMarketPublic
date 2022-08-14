using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _4AutoMarket.Models
{
    public class IdramPaymentHistory
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public string TransactionDate { get; set; }
        public string TransactionID { get; set; }
        public string OrderNumber { get; set; }
        public string IdramID { get; set; }
        public string PayerIdramId { get; set; }
        public string OrderPayedSum { get; set; }
        public string CheckSum { get; set; }
    }
}
