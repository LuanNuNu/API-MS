using MenShop_Assignment.Datas;
using MenShop_Assignment.Models;

namespace MenShop_Assignment.Mapper
{
    public static class DiscountPriceMapper
    {
        public static DiscountPriceViewModel ToDiscountPriceViewModel(DiscountPrice discountPriceViewModel)
        {
            return new DiscountPriceViewModel
            {
                Id = discountPriceViewModel.Id,
                Name = discountPriceViewModel.Name,
                Description = discountPriceViewModel.Description,
                DiscountPercent = discountPriceViewModel.DiscountPercent,
                StartTime = discountPriceViewModel.StartTime,
                EndTime = discountPriceViewModel.EndTime,
                IsActive = discountPriceViewModel.IsActive
            };
        }


        public static DiscountPriceDetailViewModel ToDiscountById(DiscountPriceDetail entity)
        {
            return new DiscountPriceDetailViewModel
            {
                Id = entity.Id,
                discountPriceId = entity.discountPriceId,
                productDetailId = entity.productDetailId,
                ProductName = entity.ProductDetail?.Product?.ProductName ?? "N/A",
                ColorName = entity.ProductDetail?.Color?.Name ?? "N/A",
                SizeName = entity.ProductDetail?.Size?.Name ?? "N/A",
                FabricName = entity.ProductDetail?.Fabric?.Name ?? "N/A"
            };
        }

        public static DiscountPriceDetailViewModel ToDiscountDetailById(DiscountPriceDetail entity)
        {
            return new DiscountPriceDetailViewModel
            {
                Id = entity.Id,
                productDetailId = entity.productDetailId,
                discountPriceId = entity.discountPriceId,
            };
        }


    }
}