namespace MenShop_Assignment.Datas
{
    public class Collection
    {
        public int CollectionId { get; set; }
        public string CollectionName { get; set; }
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public ICollection<ImageCollection>? Images { get; set; }
        public bool Status { get; set; }

        public ICollection<CollectionDetail>? CollectionDetails { get; set; }

    }
}
