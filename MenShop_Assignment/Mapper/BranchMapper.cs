using MenShop_Assignment.Datas;
using MenShop_Assignment.DTOs;
using MenShop_Assignment.Models;

namespace MenShop_Assignment.Mapper
{
    public static class BranchMapper
    {
        public static BranchViewModel ToBranchViewModel(Branch branch)
        {
            return new BranchViewModel
            {
                BranchId = branch.BranchId,
                Name = branch.Name,
                Address = branch.Address,       
                ManagerName = branch.Manager?.FullName ?? null,
                IsOnline = branch.IsOnline,
                BranchDetails = branch.BranchDetails?.Select(ToBranchDetailViewModel).ToList() ?? [],
            };
        }
        public static ProductDetailViewModel ToBranchDetailViewModel(BranchDetail branchDetail)
        {
            var now = DateTime.Now;
            var activeDiscount = branchDetail.ProductDetail.DiscountPriceDetails?
                .FirstOrDefault(x => x.DiscountPrice != null
                && x.DiscountPrice.IsActive == true)
                ?.DiscountPrice;

            return new ProductDetailViewModel
            {
                DetailId = branchDetail.ProductDetailId,
                ProductName = branchDetail.ProductDetail?.Product?.ProductName,
                FabricName = branchDetail.ProductDetail?.Fabric?.Name,
                ColorName = branchDetail.ProductDetail?.Color?.Name,
                SizeName = branchDetail.ProductDetail?.Size?.Name,
                SellPrice = branchDetail.Price,
                Quantity = branchDetail.Quantity,
                DiscountPercent = activeDiscount?.DiscountPercent ?? 0,
                DiscountedPrice = activeDiscount != null
                    ? branchDetail.Price * (1 - activeDiscount.DiscountPercent / 100)
                    : branchDetail.Price,
                Images = branchDetail.ProductDetail?.Images?.Select(x => x.FullPath).ToList() ?? []
            };
        }
        public static CartPriceDetailViewModel ToCartPriceDetailViewModel(BranchDetail branchDetail)
        {
            var now = DateTime.Now;
            var activeDiscount = branchDetail.ProductDetail.DiscountPriceDetails?
                .FirstOrDefault(x => x.DiscountPrice != null && x.DiscountPrice.IsActive == true)
                ?.DiscountPrice;

            return new CartPriceDetailViewModel
            {
                DetailId = branchDetail.ProductDetailId,
                SellPrice = branchDetail.Price,
                DiscountPercent = activeDiscount?.DiscountPercent ?? 0,
                DiscountedPrice = activeDiscount != null
                    ? branchDetail.Price * (1 - activeDiscount.DiscountPercent / 100)
                    : branchDetail.Price,
                Quantity = branchDetail.Quantity
            };
        }

        public static CreateUpdateBranchDTO ToDTO(Branch branch)
        {
            return new CreateUpdateBranchDTO
            {
                Address = branch.Address,
                Name = branch.Name,
                IsOnline = branch.IsOnline
            };
        }
        public static Branch ToBranch(CreateUpdateBranchDTO dto)
        {
            return new Branch
            {
                Address = dto.Address,
                Name = dto.Name,
                IsOnline = dto.IsOnline
            };
        }

    }
}
