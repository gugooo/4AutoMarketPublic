using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _4AutoMarket.ViewModels.Admin
{
    public class HomePageSectionsView
    {
        public HomePageSectionsView()
        {

        }
        public HomePageSectionsView(bool firstLoad)
        {
            var TempSections = new SectionView[7];
            TempSections[0] = new SectionView("Section 1");
            TempSections[1] = new SectionView("Section 2", 1);
            TempSections[2] = new SectionView("Section 3");
            TempSections[3] = new SectionView("Section 4", 2);
            TempSections[4] = new SectionView("Section 5", 3);
            TempSections[5] = new SectionView("Section 6", 9);
            TempSections[6] = new SectionView("Section 7", 1);
            this.Sections = TempSections;
        }
        public IEnumerable<SectionView> Sections { get; set; }
        public int[][] ChangedImgs { get; set; }
        public IEnumerable<IFormFile> Imgs_0 { get; set; }
        public IEnumerable<IFormFile> Imgs_1 { get; set; }
        public IEnumerable<IFormFile> Imgs_2 { get; set; }
        public IEnumerable<IFormFile> Imgs_3 { get; set; }
        public IEnumerable<IFormFile> Imgs_4 { get; set; }
        public IEnumerable<IFormFile> Imgs_5 { get; set; }
        public IEnumerable<IFormFile> Imgs_6 { get; set; }
        public IEnumerable<IFormFile> GetImgs(int Index)
        {
            switch (Index)
            {
                case 0: return Imgs_0;
                case 1:
                    return Imgs_1;
                case 2:
                    return Imgs_2;
                case 3:
                    return Imgs_3;
                case 4:
                    return Imgs_4;
                case 5:
                    return Imgs_5;
                case 6:
                    return Imgs_6;
                default: return null;
            }
        }
    }
    public class SectionView
    {
        public SectionView() { }
        public SectionView(string Name, int? FixedData = null)
        {
            this.FixedData = FixedData;
            if (FixedData != null) { SectionDatas = new SectionDataView[(int)FixedData]; this.IsFixedData = true; }
            else SectionDatas = new SectionDataView[1];
            this.SectionName = Name;
            this.Id = "first";
        }
        public string Id { get; set; }
        public string SectionName { get; set; }
        public int Position { get; set; }
        public bool IsActive { get; set; }
        private int? FixedData { get; }
        public bool IsFixedData { get; set; }

        public IEnumerable<SectionDataView> SectionDatas { get; set; }
    }
    public class SectionDataView
    {
        public SectionDataView()
        {
            this.IsDelete = false;
        }
        public string Id { get; set; }
        public string ImgID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool TextIsWhith { get; set; }
        public bool IsDelete { get; set; }
        public string CatalogId { get; set; }
    }
}
