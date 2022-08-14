using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace _4AutoMarket.ViewModels
{
    public class PromoCodeViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        [Range(0, 100)]
        public decimal Parcent { get; set; }
        [Required]
        public DateTime Expired { get; set; }

    }
}
