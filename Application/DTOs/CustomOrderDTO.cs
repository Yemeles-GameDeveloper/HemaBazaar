using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class CustomOrderDTO
    {
        public int OrderId { get; set; }
        public string BandColor { get; set; }
        public string ThreadColor { get; set; }
        public string LaceColor { get; set; }
        public string CategoryName { get; set; }
        public string Model { get; set; }
        public string Size { get; set; }
    }
}
