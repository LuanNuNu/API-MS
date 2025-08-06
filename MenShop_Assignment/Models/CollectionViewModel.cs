using MenShop_Assignment.Datas;

namespace MenShop_Assignment.Models
{
    public class CollectionViewModel
    {
        public int CollectionId { get; set; }
        public string CollectionName { get; set; }
        public string? Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool Status { get; set; }
        public ICollection<ImageCollection>? Images { get; set; }
        public List<CollectionDetailsViewModel> CollectionDetails { get; set; } = new();
    }
}
