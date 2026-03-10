namespace HemaBazaar.MVC.Models
{
    public class CartRequestModel
    {
        public int ItemId { get; set; }
        public int UserId { get; set; }
        public int Quantity { get; set; }
    }
}
