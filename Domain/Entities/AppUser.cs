using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class AppUser : IdentityUser<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }


        public ICollection<Cart> Carts { get; set; }
        public ICollection<Favourite> Favourites { get; set; }
        public ICollection<Purchase> Purchases { get; set; }
        public ICollection<AuditLog> AuditLogs { get; set; }



    }
}
