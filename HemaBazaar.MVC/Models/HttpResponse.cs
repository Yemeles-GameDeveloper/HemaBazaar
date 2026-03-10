namespace HemaBazaar.MVC.Models
{
    public class HttpResponse
    {
        public bool IsSuccessStatusCode { get; set; }
        public int StatusCode { get; set; }
        public string? Content { get; set; }
    }
}
