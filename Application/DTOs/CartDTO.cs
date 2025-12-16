using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class CartDTO
    {
        public int Id { get; set; }
        public int AppUserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }
        public int Quantity { get; set; }
        public int ItemId { get; set; }
        public bool IsActive { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get => Price * Quantity; }


    }
}
