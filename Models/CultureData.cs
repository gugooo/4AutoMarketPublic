using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace _4AutoMarket.Models
{
    public class CultureData
    {
        public static string GetDefoultName(ICollection<CultureData> cultureDatas, string Culture, string ifEmpty = "")
        {
            if (cultureDatas == null || cultureDatas.Count == 0 || string.IsNullOrEmpty(Culture)) return ifEmpty;
            var currentCulture = cultureDatas.FirstOrDefault(_ => _.Culture.ToUpper() == Culture.ToUpper() && !string.IsNullOrEmpty(_.Text));
            if (currentCulture == null)
            {
                var OtherCulture = cultureDatas.FirstOrDefault(_ => !string.IsNullOrEmpty(_.Text));
                if (OtherCulture == null) return ifEmpty;
                return OtherCulture.Text;
            }
            return currentCulture.Text;
        }
        public static ICollection<CultureData> SetOrChangeCultureName(ICollection<CultureData> cultureDatas, string NewText, string Culture)
        {
            if (string.IsNullOrEmpty(Culture))
            {
                if (cultureDatas == null) return new HashSet<CultureData>();
                return cultureDatas;
            }
            if (cultureDatas == null) cultureDatas = new HashSet<CultureData>() { new CultureData() { Culture = Culture, Text = NewText ?? "" } };
            else
            {
                var TempCurrent = cultureDatas.FirstOrDefault(_ => _.Culture.ToUpper() == Culture.ToUpper());
                if (TempCurrent == null) cultureDatas.Add(new CultureData() { Culture = Culture, Text = NewText ?? "" });
                else TempCurrent.Text = NewText ?? "";
            }
            return cultureDatas;
        }
        public string Id { get; set; }
        public string Culture { get; set; }
        public string Text { get; set; }

        public virtual ProductType FK_Title { get; set; }
        public virtual ProductType FK_Brand { get; set; }
        public virtual ProductType FK_Description { get; set; }
        public virtual HomePageSectionData FK_HP_Title { get; set; }
        public virtual HomePageSectionData FK_HP_Description { get; set; }

    }
}
