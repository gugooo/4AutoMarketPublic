using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace _4AutoMarket.Models
{
    public class Product
    {
        public Product()
        {
            ProductTypes = new HashSet<ProductType>();
            Reviews = new HashSet<Review>();
        }
        public string Id { get; set; }
        public int _Index { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<ProductType> ProductTypes { get; set; }
        public virtual Atribute LinkAtribute { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
    }
    public class ProductType
    {
        public ProductType()
        {
            CultureTitle = new HashSet<CultureData>();
            CultureDescription = new HashSet<CultureData>();
            CultureBrand = new HashSet<CultureData>();
            Images = new HashSet<ProductIMG>();
            ProductAtributes = new HashSet<ProductAtributes>();
        }
        public string Id { get; set; }
        public int _Index { get; set; }
        public virtual ICollection<CultureData> CultureBrand { get; set; }
        public virtual ICollection<CultureData> CultureTitle { get; set; }
        public virtual ICollection<CultureData> CultureDescription { get; set; }
        public string SerchKeys { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Price { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Sale { get; set; }
        public bool IsActive { get; set; }
        public bool IsMine { get; set; }
        public virtual ICollection<ProductIMG> Images { get; set; }
        public virtual ICollection<ProductAtributes> ProductAtributes { get; set; }
        public virtual AtributeValue LinkAtributeValue { get; set; }
        public virtual Atribute FirstAtribute { get; set; }
    }
    public class ProductAtributes
    {
        public string Id { get; set; }
        public ProductAtributes()
        {
            AtributeValues = new HashSet<ProductAtributeValue>();
        }
        public virtual ICollection<ProductAtributeValue> AtributeValues { get; set; }
        public int ProductQuantity { get; set; }
    }
    public class ProductAtributeValue
    {
        public string Id { get; set; }
        public int _Index { get; set; }
        public virtual AtributeValue AtributeValue { get; set; }
    }
    public class ProductIMG
    {
        public string Id { get; set; }
        public int _Index { get; set; }
        public byte[] IMG { get; set; }
    }
}
