using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace _4AutoMarket.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Required")]
        [MaxLength(50, ErrorMessage = "MaxLenght")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "InvalidEmail")]
        public string ForgotPasswordEmail { get; set; }
    }
}
