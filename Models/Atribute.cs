using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace _4AutoMarket.Models
{
    public class Atribute
    {
        public Atribute()
        {
            CultureName = new HashSet<CultureData>();
            Values = new HashSet<AtributeValue>();
        }
        public string Id { get; set; }
        public int _Index { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<CultureData> CultureName { get; set; }
        public virtual ICollection<AtributeValue> Values { get; set; }
    }
    public class AtributeValue
    {
        public AtributeValue()
        {
            CultureName = new HashSet<CultureData>();
        }
        public string Id { get; set; }
        public int _Index { get; set; }
        public string Value { get; set; }
        public virtual ICollection<CultureData> CultureName { get; set; }
        public virtual Atribute FK_Atribute { get; set; }
    }
}
