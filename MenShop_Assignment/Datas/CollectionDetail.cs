namespace MenShop_Assignment.Datas
{
    public class CollectionDetail
    {
        public int CollectionDetailId { get; set; } 
        public int CollectionId { get; set; }
        public Collection? Collection { get; set; }

        public int ProductId { get; set; }
        public Product? Product { get; set; }

    }
}
