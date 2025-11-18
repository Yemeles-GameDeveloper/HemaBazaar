using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    internal class Purchase : BaseEntity
    {
        public int AppUserId { get; set; }
        public int ItemId { get; set; }
        public int PaymentId { get; set; }
        public DateTime PurchaseDate { get; set; } = DateTime.Now;


        public AppUser AppUser { get; set; }
        public Item Item { get; set; }
        public Payment Payment { get; set; }



    }
}
