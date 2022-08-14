using _4AutoMarket.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _4AutoMarket.ViewModels
{
    public class ProductBagViewModel
    {
        public string ProductId { get; set; }
        public string Title { get; set; }
        public string ImgId { get; set; }
        public int Quantity { get; set; }
        public int MaxQuantity { get; set; }
        public string Price { get; set; }
        public string Sale { get; set; }
        public decimal dSale { get; set; }
        public decimal dPrice { get; set; }

        public IEnumerable<AtributeValue> Atributes { get; set; }
    }
}
