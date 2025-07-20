namespace MenShop_Assignment.DTOs
{
    public class CreateUpdateCustomerAddressDTO
    {
        public int Id { get; set; } = 0;
        public string? CustomerId { get; set; }
        public string? Address { get; set; }
    }

}
