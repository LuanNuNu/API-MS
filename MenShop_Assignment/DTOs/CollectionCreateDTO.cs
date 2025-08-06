namespace MenShop_Assignment.DTOs
{
    public class CollectionCreateDTO
    {
        public string CollectionName { get; set; }
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool Status { get; set; }
    }
}
