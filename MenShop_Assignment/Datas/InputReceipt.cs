using MenShop_Assignment.Extensions;

namespace MenShop_Assignment.Datas
{
    public class InputReceipt
    {
        public int ReceiptId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? CancelDate { get; set; }
        public DateTime? ConfirmedDate { get; set; }
        public OrderStatus? Status { get; set; }
        public decimal? Total {  get; set; }
        public string? ManagerId { get; set; }
        public User? Manager { get; set; }
        public int StorageId { get; set; }
        public Storage? Storage { get; set; }
        public ICollection<InputReceiptDetail>? InputReceiptDetails { get; set; }
    }
}
