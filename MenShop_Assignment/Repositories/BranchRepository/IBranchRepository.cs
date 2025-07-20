using MenShop_Assignment.Datas;
using MenShop_Assignment.DTOs;
using MenShop_Assignment.Models;
using System.Threading.Tasks;

namespace MenShop_Assignment.Repositories.BranchesRepository
{
    public interface IBranchRepository
    {
        Task<List<BranchViewModel>?> GetBranchesAsync();
        Task<BranchViewModel?> GetBranchByIdAsync(int Id);
        Task<Branch?> CreateBranchAsync(CreateUpdateBranchDTO branchDTO);
        Task<Branch?> UpdateBranchAsync(int branchId, CreateUpdateBranchDTO branchDTO);
        Task<List<ProductViewModel>?> GetBranchProductsAsync(int? branchId, int? categoryId, string? role);
        Task<List<ProductDetailViewModel>?> GetDetailProductBranchAsync(int? branchId, int productId, string? role);
        Task<List<ProductViewModel>> SmartSearchProductsAsync(int branchId, string nameLike, int? idMatch);
        Task<bool> DeleteBranchAsync(int branchId);

    }
}
