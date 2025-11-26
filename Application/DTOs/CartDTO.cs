using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class CartDTO
    {
        public int AppUserId { get; set; }
        public string ItemTitle { get; set; }
        public string UserName { get; set; }
        public int Quantity { get; set; }

    }
}
