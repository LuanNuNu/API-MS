namespace MenShop_Assignment.Datas
{
    //up
    public class Cart
    {
        public int CartId { get; set; }
        public string? CustomerId {  get; set; }
        public string? AnonymousId { get; set; }
        public User? Customer { get; set; }
        public DateTime? CreatedDate { get; set; }
        public ICollection<CartDetail>? Details { get; set; }
    }
}
