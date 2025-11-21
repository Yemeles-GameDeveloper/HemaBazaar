using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Order : BaseEntity
    {
        //bağlantı ekle
        public int AppUserId { get; set; }
        
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public string Address { get; set; }


        public AppUser AppUser { get; set; }      
        public ICollection<OrderDetail> orderDetails { get; set; }
        public ICollection<CustomOrder> CustomOrders { get; set; }

    }
}
