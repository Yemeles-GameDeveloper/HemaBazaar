using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Item : BaseEntity
    {
        //Spesifik zırh ve silahları ekle
        public string Title { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public int CategoryId { get; set; }


        public ICollection<Cart> Carts { get; set; }
        public ICollection<Favourite> Favourites { get; set; }
        public Category Category { get; set; }
        public ICollection<Purchase> Purchases { get; set; }

        public ICollection<OrderDetail> OrderDetails  { get; set; }

    }
}
