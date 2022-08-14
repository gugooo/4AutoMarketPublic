using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace _4AutoMarket.ViewModels
{
    public class CheckoutViewModel
    {
        [Display(Name = "Phone")]
        [Required(ErrorMessage = "SharedRequired")]
        [MaxLength(20, ErrorMessage = "SharedMaxLenght")]
        public string Phone { get; set; }

        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "InvalidEmail")]
        public string Email { get; set; }

        [Display(Name = "Address")]
        [Required(ErrorMessage = "SharedRequired")]
        [MaxLength(100, ErrorMessage = "SharedMaxLenght")]
        public string Address { get; set; }

        [Display(Name = "Building")]
        [MaxLength(20, ErrorMessage = "SharedMaxLenght")]
        public string Building { get; set; }

        [Display(Name = "Apartment")]
        [MaxLength(20, ErrorMessage = "SharedMaxLenght")]
        public string Apt { get; set; }

        [Display(Name = "Enter")]
        [MaxLength(20, ErrorMessage = "SharedMaxLenght")]
        public string Enter { get; set; }

        [Display(Name = "Floor")]
        [MaxLength(20, ErrorMessage = "SharedMaxLenght")]
        public string Floor { get; set; }

        [Display(Name = "Code")]
        [MaxLength(20, ErrorMessage = "SharedMaxLenght")]
        public string Code { get; set; }

        [Display(Name = "Description")]
        [MaxLength(200, ErrorMessage = "SharedMaxLenght")]
        public string Description { get; set; }
        public string PromoCode { get; set; }
        public string Payment { get; set; }
    }
}
