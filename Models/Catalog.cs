using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;


namespace _4AutoMarket.Models
{
    public class Catalog
    {
        public Catalog()
        {
            this.CultureName = new HashSet<CultureData>();
            this.Products = new HashSet<Product>();
            this.ChaildCatalogs = new HashSet<Catalog>();
        }
        public string Id { get; set; }
        public int _Index { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<CultureData> CultureName { get; set; }
        public virtual ICollection<Catalog> ChaildCatalogs { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public int Position { get; set; }

        #region FOREIGN KEY
        public string FatherCatalogId { get; set; }
        public virtual Catalog FatherCatalog { get; set; }
        #endregion

    }
}
