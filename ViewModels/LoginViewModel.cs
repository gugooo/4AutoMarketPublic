using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace _4AutoMarket.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "SharedRequired")]
        [MaxLength(50, ErrorMessage = "MaxLenght")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "InvalidEmail")]
        public string LoginEmail { get; set; }

        [Required(ErrorMessage = "SharedRequired")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [MaxLength(25, ErrorMessage = "SharedMaxLenght")]
        [MinLength(6, ErrorMessage = "SharedMinLenght")]
        public string LoginPassword { get; set; }

        [Display(Name = "Remember")]
        public bool Remember { get; set; }
    }
}
