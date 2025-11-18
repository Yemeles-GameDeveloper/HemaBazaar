using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Favourite : BaseEntity
    {
        public int AppUserId { get; set; }
        public int ItemId { get; set; }



        public AppUser AppUser { get; set; }
        public Item Item { get; set; }

    }
}
