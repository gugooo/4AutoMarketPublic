using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _4AutoMarket.Models
{
    public class ProductAjax
    {
        public string CatalogId { get; set; }
        public string ProductId { get; set; }
        public string ProductTypeId { get; set; }
        public string Title { get; set; }
        public string Brand { get; set; }
        public string Description { get; set; }
        public string SerchKeys { get; set; }
        public string Price { get; set; }
        public string Sale { get; set; }
        public bool IsActive { get; set; }
        public bool IsMine { get; set; }
        public IEnumerable<string> RemoveIMGsId { get; set; }
        public IEnumerable<ProductAtributesAjax> ProductAtributes { get; set; }
        public string CurrentAtributeValue { get; set; }
        public string GroupAtribute { get; set; }
        public IEnumerable<string> AtributeLinks { get; set; }
        public string FirstAtributeId { get; set; }
    }
    public class ProductAtributesAjax
    {
        public IEnumerable<string> AtributeValuesID { get; set; }
        public int Quantity { get; set; }
    }
}
