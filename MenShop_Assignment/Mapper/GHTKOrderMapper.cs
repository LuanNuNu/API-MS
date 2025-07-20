using MenShop_Assignment.Datas;
using MenShop_Assignment.Models;
using MenShop_Assignment.Models.GHTKModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client.Extensions.Msal;

namespace MenShop_Assignment.Mapper
{
    public class GHTKOrderMapper
    {
        private readonly ApplicationDbContext _context;
        public GHTKOrderMapper(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<GHTKOrderViewModel> ToGHTKOrderView(GHTKOrder order)
        {
            var product = await _context.GHTKProducts
                .Where(x => x.ProductId == order.ProductId)
                .FirstOrDefaultAsync();

            return new GHTKOrderViewModel
            {
                Id = order.Id,
                ProductName = product?.Name ?? string.Empty,
                Name = order.Name,
                Address = order.Address,
                District = order.District,
                Email = order.Email,
                PickAddress = order.PickAddress,
                PickDistrict = order.PickDistrict,
                PickMoney = order.PickMoney,
                PickName = order.PickName,
                PickProvince = order.PickProvince,
                PickTel = order.PickTel,
                Province = order.Province,
                Ward = order.Ward,
                Street = order.Street,
                Tel = order.Tel,
                ReturnName = order.ReturnName,
                ReturnEmail = order.ReturnEmail,
                ReturnAddress = order.ReturnAddress,
                ReturnDistrict = order.ReturnDistrict,
                ReturnStreet = order.ReturnStreet,
                ReturnProvine = order.ReturnProvine,
                ReturnTel = order.ReturnTel,
            };
        }
    }
}
