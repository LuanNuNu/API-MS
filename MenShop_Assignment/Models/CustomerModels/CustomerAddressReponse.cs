namespace MenShop_Assignment.Models.CustomerModels
{
    public class CustomerAddressReponse
    {
        public int Id { get; set; }
        public string? CustomerId { get; set; } 
        public string? Address { get; set; }
        public bool Success { get; set; }
        public string? Message { get; set; }
    }
}
