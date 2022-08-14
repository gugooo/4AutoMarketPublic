using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace _4AutoMarket.Models
{
    public class User : IdentityUser
    {
        public User()
        {
            this.AddressBook = new HashSet<AddressBook>();
            this.UserSend = new HashSet<Message>();
            this.AdminSend = new HashSet<Message>();
            this.AdminSharedMessage = new HashSet<SharedMessageUser>();
            this.Reviews = new HashSet<Review>();
        }
        public int _Index { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime Registred { get; set; }
        public string Document { get; set; }
        public string Description { get; set; }
        //-----------------------------------------------
        public string Country { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string PostCode { get; set; }
        //-----------------------------------------------
        public virtual ICollection<AddressBook> AddressBook { get; set; }
        public virtual AddressBook SelectedAddress { get; set; }
        //-----------------------------------------------
        public virtual ICollection<Message> UserSend { get; set; }
        public virtual ICollection<Message> AdminSend { get; set; }
        public virtual ICollection<SharedMessageUser> AdminSharedMessage { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
