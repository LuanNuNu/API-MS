namespace MenShop_Assignment.Datas
{
    public class ImageCollection : Image
    {
        public int CollectionId { get; set; }
        public Collection? Collection { get; set; }
    }
}
