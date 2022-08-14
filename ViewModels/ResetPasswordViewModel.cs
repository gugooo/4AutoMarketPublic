using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace _4AutoMarket.ViewModels
{
    public class ResetPasswordViewModel
    {

        [Required(ErrorMessage = "SharedRequired")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [MaxLength(25, ErrorMessage = "SharedMaxLenght")]
        [MinLength(6, ErrorMessage = "SharedMinLenght")]
        //[RegularExpression(@"^(?=.*[a-zа-яա-ֆ])(?=.*[A-ZА-ЯԱ-Ֆ])(?=.*\d)\S{6,20}$", ErrorMessage = "PasswordRegEx")]
        public string ResetPassword { get; set; }

        [Required(ErrorMessage = "SharedRequired")]
        [DataType(DataType.Password)]
        [Compare("ResetPassword", ErrorMessage = "ComparePasswordErrMassage")]
        [Display(Name = "ComparePassword")]
        [MaxLength(25, ErrorMessage = "SharedMaxLenght")]
        [MinLength(6, ErrorMessage = "SharedMinLenght")]
        //[RegularExpression(@"^(?=.*[a-zа-яա-ֆ])(?=.*[A-ZА-ЯԱ-Ֆ])(?=.*\d)\S{6,20}$", ErrorMessage = "PasswordRegEx")]
        public string CompareResetPassword { get; set; }

        [Required]
        [MaxLength(100)]
        public string EmailID { get; set; }

        [Required]
        [MaxLength(300)]
        public string Code { get; set; }
    }
}
