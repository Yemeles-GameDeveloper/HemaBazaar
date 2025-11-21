using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class OrderDetail : BaseEntity
    {
        
        public int OrderId { get; set; }
        public int ItemId { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }


        public Order Order { get; set; }
        public Item Item { get; set; }
        


    }
}
