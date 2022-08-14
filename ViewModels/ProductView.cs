using _4AutoMarket.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace _4AutoMarket.ViewModels
{
    public class ProductView
    {
        public string Id { get; set; }
        public string ImgId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Rating { get; set; }
        public string Price { get; set; }
        public string Sale { get; set; }
        public string Brand { get; set; }
        public IDictionary<Atribute, List<AtributeValue>> Atributes { get; set; }
        public IEnumerable<string> Imgs { get; set; }
        public AtributeValue LinkAtrVal { get; set; }
    }
}
