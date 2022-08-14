using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace _4AutoMarket.ViewModels.MyAccount
{
    public class AddAddressBookViewModel
    {
        [Required(ErrorMessage = "SharedRequired")]
        [Display(Name = "FName")]
        [MaxLength(50, ErrorMessage = "SharedMaxLenght")]
        [DataType(DataType.Text)]
        [RegularExpression(@"^[А-Яа-яԱ-Ֆա-ֆA-Za-z-\s]{3,50}$", ErrorMessage = "ValidName")]
        public string FName { get; set; }

        [Required(ErrorMessage = "SharedRequired")]
        [Display(Name = "LName")]
        [MaxLength(50, ErrorMessage = "SharedMaxLenght")]
        [DataType(DataType.Text)]
        [RegularExpression(@"^[А-Яа-яԱ-Ֆա-ֆA-Za-z-\s]{3,50}$", ErrorMessage = "ValidName")]
        public string LName { get; set; }

        [Required(ErrorMessage = "SharedRequired")]
        [MaxLength(50, ErrorMessage = "SharedMaxLenght")]
        [DataType(DataType.Text)]
        [Display(Name = "Country")]
        [RegularExpression(@"^[А-Яа-яԱ-Ֆա-ֆA-Za-z-\s]{3,50}$", ErrorMessage = "ValidName")]
        public string Country { get; set; }

        [Required(ErrorMessage = "SharedRequired")]
        [MaxLength(50, ErrorMessage = "SharedMaxLenght")]
        [DataType(DataType.Text)]
        [Display(Name = "City")]
        [RegularExpression(@"^[А-Яа-яԱ-Ֆա-ֆA-Za-z-\s]{3,50}$", ErrorMessage = "ValidName")]
        public string City { get; set; }

        [Required(ErrorMessage = "SharedRequired")]
        [MaxLength(50, ErrorMessage = "SharedMaxLenght")]
        [DataType(DataType.Text)]
        [Display(Name = "Adress")]
        public string Adress { get; set; }

        [Display(Name = "Postcode")]
        [DataType(DataType.PostalCode)]
        public string Postcode { get; set; }
    }
}
