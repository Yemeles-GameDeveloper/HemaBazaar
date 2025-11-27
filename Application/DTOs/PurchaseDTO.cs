using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class PurchaseDTO
    {
        public int AppUserId { get; set; }
        public string UserName { get; set; }
        public string ItemTitle { get; set; }
        public DateTime PurchaseDate { get; set; } = DateTime.Now;
    }
}
