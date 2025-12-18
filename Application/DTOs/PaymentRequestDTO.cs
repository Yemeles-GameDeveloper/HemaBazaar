using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class PaymentRequestDTO
    {
        public decimal Price { get; set; }
        public decimal PaidPrice { get; set; }
        public string BasketId { get; set; }
        public int UserId { get; set; }

    }
}
