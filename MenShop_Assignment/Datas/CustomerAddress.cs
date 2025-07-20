namespace MenShop_Assignment.Datas
{
    public class CustomerAddress
    {
        public int Id { get; set; }
        public string? CustomerId { get; set; }
        public string? Address { get; set; }
        public User? Customer { get; set; }
    }
}
