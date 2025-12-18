using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels
{
    public class CheckoutViewModel
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string ZIP { get; set; }
        public string DeliveryInstructions { get; set; }
        public decimal Price { get; set; }
        public decimal PaidPrice { get; set; }

        public IEnumerable<CartDTO> CartItems { get; set; } = Enumerable.Empty<CartDTO>();

    }
}
