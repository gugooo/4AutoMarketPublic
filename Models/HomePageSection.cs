using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace _4AutoMarket.Models
{
    public class HomePageSection
    {
        public HomePageSection()
        {
            this.HomePageSectionDatas = new HashSet<HomePageSectionData>();
        }
        public string Id { get; set; }
        public int _Index { get; set; }
        public bool IsActive { get; set; }
        public bool IsFixedData { get; set; }
        public int Position { get; set; }
        public string SectionName { get; set; }
        public virtual ICollection<HomePageSectionData> HomePageSectionDatas { get; set; }
    }
    public class HomePageSectionData
    {
        public HomePageSectionData()
        {
            this.Title = new HashSet<CultureData>();
            this.Description = new HashSet<CultureData>();
            this.Added = DateTime.Now;
        }
        public string Id { get; set; }
        public int _Index { get; set; }
        public byte[] Img { get; set; }
        public DateTime Added { get; set; }
        public virtual ICollection<CultureData> Title { get; set; }
        public virtual ICollection<CultureData> Description { get; set; }
        public bool TextIsWhith { get; set; }
        public virtual Catalog Catalog { get; set; }
        public virtual HomePageSection FK_HomePageSection { get; set; }

    }
}
