using MenShop_Assignment.Datas;

namespace MenShop_Assignment.DTOs
{
    public class UpdateHistoryPriceDTO
    {

        public int ProductDetailId { get; set; }
        public ProductDetail? ProductDetail { get; set; }
        public decimal? InputPrice { get; set; }
        public decimal? SellPrice { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
