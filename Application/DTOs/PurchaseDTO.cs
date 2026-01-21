using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class PurchaseDTO
    {
        public int Id { get; set; }
        public int AppUserId { get; set; }
        public string UserName { get; set; }
        public string ItemTitle { get; set; }
        public DateTime PurchaseDate { get; set; } = DateTime.Now;
        public bool IsActive { get; set; }
        public int ItemId { get; set; }
        public int PaymentId { get; set; }

        public int TransactionId { get; set; }
        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; }








    }
}
