using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class PaymentDTO
    {
        public decimal Amount { get; set; }
        public string TransactionId { get; set; }
        public DateTime PaymentDay { get; set; }
        public PaymentStatus Status { get; set; }


    }
}
