using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace _4AutoMarket.ViewModels
{
    public class IndexViewModel
    {
        public IndexViewModel()
        {
            this.checkoutViewModel = new CheckoutViewModel();
            this.loginViewModel = new LoginViewModel();
            this.registrationViewModel = new RegistrationViewModel();
        }
        public LoginViewModel loginViewModel { get; set; }
        public RegistrationViewModel registrationViewModel { get; set; }
        public CheckoutViewModel checkoutViewModel { get; set; }
    }
}
