namespace MenShop_Assignment.DTOs
{
    public class CreateReceiptDetailDTO
    {
        public int? ProductId { get; set; }
        public int? ColorId { get; set; }
        public int? SizeId { get; set; }
        public int? FabricId { get; set; }
        public decimal? Price { get; set; }
        public int? Quantity { get; set; }
    }
}
