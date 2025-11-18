using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Payment : BaseEntity
    {
        public decimal Amount { get; set; }
        public int TransactionId { get; set; }
        public DateTime PaymentDay { get; set; }
        public PaymentStatus Status { get; set; }


        public ICollection<Purchase> Purchases { get; set; }
    }
}
