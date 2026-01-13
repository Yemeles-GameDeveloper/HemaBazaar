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
        public string TransactionId { get; set; }
        public DateTime PaymentDay { get; set; }
        public PaymentStatus Status { get; set; }
        
        public int AppUserId { get; set; }


        public ICollection<Purchase> Purchases { get; set; }
        public AppUser AppUser { get; set; }

        // 26 Kasım 1:04:00
    }
}
