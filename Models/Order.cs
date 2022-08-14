using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace _4AutoMarket.Models
{
    public class Order
    {
        public Order()
        {
            this.ProductsInfo = new HashSet<OrderProductInfo>();
        }
        public string Id { get; set; }
        public Int64 OrderNumber { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime Created { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal ProductsSum { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal ShipingSum { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal PromoSale { get; set; }

        public string PromoCode { get; set; }
        public virtual ICollection<OrderProductInfo> ProductsInfo { get; set; }
        public enum StatusType { Pending, Shipping, Canceled, Refunded, Delivered }
        public string Status { get; set; }
        public virtual User User { get; set; }

    }
    public class OrderProductInfo
    {
        public OrderProductInfo()
        {
            this.AtributeAndValue = new HashSet<OrderAtributeValue>();
        }
        public int Id { get; set; }
        public string ProductImgId { get; set; }
        public string ProductTitle { get; set; }
        public string Brand { get; set; }
        public string ProductDescription { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal ProductSum { get; set; }
        public int Quantity { get; set; }

        public virtual ICollection<OrderAtributeValue> AtributeAndValue { get; set; }
        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
        public virtual ProductType ProductType { get; set; }

        public virtual ProductAtributes ProductAtributes { get; set; }
        public string GetTitle(string Culture)
        {
            if (this.ProductType != null)
            {
                return CultureData.GetDefoultName(ProductType.CultureTitle, Culture);
            }
            if (this.Product != null)
            {
                var main = Product.ProductTypes.FirstOrDefault(_ => _.IsMine);
                if (main != null) return CultureData.GetDefoultName(main.CultureTitle, Culture);
            }
            return ProductTitle ?? "";
        }
    }
    public class OrderAtributeValue
    {
        public int Id { get; set; }
        public string Atribute { get; set; }
        public string Value { get; set; }
        public virtual AtributeValue AtributeValue { get; set; }
        public virtual OrderProductInfo ProductInfo { get; set; }
    }
}
