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
                Address = branch.Address ?? null,
                ManagerName = branch.Manager?.FullName ?? null,
                IsOnline = branch.IsOnline,
                BranchDetails = branch.BranchDetails?.Select(ToBranchDetailViewModel).ToList() ?? [],
            };
        }
        public static ProductDetailViewModel ToBranchDetailViewModel(BranchDetail branchDetail)
        {
            return new ProductDetailViewModel
            {
                DetailId = branchDetail.ProductDetailId,
                ProductName = branchDetail.ProductDetail?.Product?.ProductName,
                FabricName = branchDetail.ProductDetail?.Fabric?.Name,
                ColorName = branchDetail.ProductDetail?.Color?.Name,
                SizeName = branchDetail.ProductDetail?.Size?.Name,
                SellPrice = branchDetail.Price,
                Quantity = branchDetail.Quantity,
                Images = branchDetail.ProductDetail?.Images?.Select(x => x.FullPath).ToList() ?? []
            };
        }

        public static Branch ToBranch(CreateUpdateBranchDTO branchDTO)
        {
            return new Branch
            {
                Address = branchDTO.Address ?? null,
                Name = branchDTO.Name,
                IsOnline = branchDTO.IsOnline
            };
        }
    }
}
