using MenShop_Assignment.Datas;
using MenShop_Assignment.Models.GHTKModel;

namespace MenShop_Assignment.Mapper
{
    public class GHTKProductMapper
    {
        public GHTKProductViewModel ToGHTKProductsOrderView(GHTKProduct product)
        {
            return new GHTKProductViewModel
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Weight = product.Weight,
            };
        }
    }
}
