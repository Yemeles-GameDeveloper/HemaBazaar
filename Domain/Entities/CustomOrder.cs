using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class CustomOrder : BaseEntity
    {
        
        public int OrderId { get; set; }
        public string BandColor { get; set; }
        public string ThreadColor { get; set; }
        public string LaceColor { get; set; }
        public int CategoryId { get; set; }
        public string Model { get; set; }
        public string Size { get; set; }


        public Order Order { get; set; }
        public Category Category { get; set; }



    }
}
