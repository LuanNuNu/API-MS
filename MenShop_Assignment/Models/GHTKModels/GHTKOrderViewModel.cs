using MenShop_Assignment.Datas;

namespace MenShop_Assignment.Models.GHTKModel
{

    public class GHTKOrderViewModel
    {
        public string Id { get; set; }
        //Người lấy hàng để giao hàng
        public string? PickName { get; set; }
        public int? PickMoney { get; set; }
        public string? PickAddress { get; set; }
        public string? PickProvince { get; set; }
        public string? PickDistrict { get; set; }
        public string? PickTel { get; set; }
        //Người nhận hàng
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Province { get; set; }
        public string? District { get; set; }
        public string? Ward { get; set; } //Phường/Xã
        public string? Street { get; set; }
        public string? Tel { get; set; }
        public string? Email { get; set; }
        //Người trả hàng
        public string? ReturnName { get; set; }
        public string? ReturnAddress { get; set; }
        public string? ReturnProvine { get; set; }
        public string? ReturnDistrict { get; set; }
        public string? ReturnStreet { get; set; }
        public string? ReturnTel { get; set; }
        public string? ReturnEmail { get; set; }
        //GHTK Products
        public string? ProductName { get; set; }
        public GHTKProduct? Products { get; set; }      
        //Status 
        public int Status { get; set; }
    }
}
