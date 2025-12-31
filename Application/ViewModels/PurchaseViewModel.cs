using System.Collections.Generic;
using Application.DTOs;

namespace HemaBazaar.MVC.Models
{
    public class PurchaseViewModel
    {
        public List<PurchaseDTO> Purchases { get; set; } = new();
    }
}
