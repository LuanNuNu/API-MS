using MenShop_Assignment.Datas;
using MenShop_Assignment.DTOs;
using MenShop_Assignment.Extensions;
using MenShop_Assignment.Models;
using MenShop_Assignment.Repositories.BranchesRepository;
using Microsoft.EntityFrameworkCore;
using MenShop_Assignment.Mapper;
namespace MenShop_Assignment.Repositories
{
    public class BranchRepository : IBranchRepository
    {
        private readonly ApplicationDbContext _context;

        public BranchRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<BranchViewModel>?> GetBranchesAsync()
        {
			var branches = await _context.Branches.Include(b=>b.Manager).ToListAsync();
            if (branches.Any())
			    return branches.Select(BranchMapper.ToBranchViewModel).ToList();
            return null;
		}
        public async Task<BranchViewModel?> GetBranchByIdAsync(int id)
        {
            var branch = await _context.Branches.Where(x=>x.BranchId == id).FirstOrDefaultAsync();
            if (branch == null) 
                return null;
            return BranchMapper.ToBranchViewModel(branch);
        }
        public async Task<Branch?> CreateBranchAsync(CreateUpdateBranchDTO branchDTO)
        {

            if (branchDTO == null)
                return null;
            var branch = BranchMapper.ToBranch(branchDTO);
			await _context.Branches.AddAsync(branch);
			await _context.SaveChangesAsync();
			return branch;
        }
        public async Task<Branch?> UpdateBranchAsync(int branchId, CreateUpdateBranchDTO branchDTO)
        {
            if (branchDTO == null)
                return null;

            var branch = await _context.Branches.FindAsync(branchId);
            if (branch == null)
                return null;


            branch.Address = branchDTO.Address;
            branch.Name = branchDTO.Name;
            branch.IsOnline = branchDTO.IsOnline;

            await _context.SaveChangesAsync();
            return branch;
        }

        public async Task<List<ProductViewModel>?> GetBranchProductsAsync(int? branchId, int? categoryId, string? role)
        {
            var isGuestOrCustomer = string.IsNullOrEmpty(role) || role == "Customer";
            int? resolvedBranchId = branchId;

            if (resolvedBranchId <= 0)
                resolvedBranchId = null;

            if (resolvedBranchId == null)
            {
                if (isGuestOrCustomer)
                {
                    var onlineBranch = await _context.Branches
                        .FirstOrDefaultAsync(b => b.IsOnline);

                    if (onlineBranch == null)
                        return null;

                    resolvedBranchId = onlineBranch.BranchId;
                }
                else
                {
                    return null;
                }
            }

            IQueryable<BranchDetail> query = _context.BranchDetails
                .Include(x => x.ProductDetail)
                    .ThenInclude(pd => pd.Product)
                        .ThenInclude(p => p.Category)
                .Include(x => x.ProductDetail.Images)
                .Include(x => x.ProductDetail.Product.CollectionDetails)
                .ThenInclude(cd => cd.Collection)
                .Where(x => x.BranchId == resolvedBranchId && x.ProductDetail.Product.Status == true);

            if (categoryId.HasValue && categoryId > 0)
            {
                query = query.Where(x => x.ProductDetail.Product.CategoryId == categoryId.Value);
            }

            var products = await query
                .GroupBy(x => x.ProductDetail.Product)
                .Select(g => new
                {
                    Product = g.Key,
                    FirstBranchDetail = g.FirstOrDefault()
                })
                .ToListAsync();

            if (!products.Any())
                return null;

            return products
                .Select(x => ProductMapper.ToBranchProductViewModel(x.Product, x.FirstBranchDetail))
                .ToList();
        }

        public async Task<List<ProductDetailViewModel>?> GetDetailProductBranchAsync(int? branchId, int productId, string? role)
        {
            var isGuestOrCustomer = string.IsNullOrEmpty(role) || role == "Customer";
            if (productId == 0)
                return null;

            int? resolvedBranchId = branchId;

            if (resolvedBranchId <= 0)
                resolvedBranchId = null;

            if (resolvedBranchId == null)
            {
                if (isGuestOrCustomer)
                {
                    var onlineBranch = await _context.Branches
                        .FirstOrDefaultAsync(b => b.IsOnline);

                    if (onlineBranch == null)
                        return null;

                    resolvedBranchId = onlineBranch.BranchId;
                }
                else
                {
                    return null;
                }
            }

            IQueryable<BranchDetail> query = _context.BranchDetails
                .Include(x => x.ProductDetail)
                    .ThenInclude(pd => pd.Product)
                .Include(x => x.ProductDetail.Color)
                .Include(x => x.ProductDetail.Size)
                .Include(x => x.ProductDetail.Fabric)
                .Include(x => x.ProductDetail.Images)
                .Include(x => x.ProductDetail.DiscountPriceDetails)
                    .ThenInclude(dp => dp.DiscountPrice)
                .Where(x => x.ProductDetail.Product.ProductId == productId)
                .Where(x => x.BranchId == resolvedBranchId);

            var branchDetails = await query.ToListAsync();

            if (branchDetails.Count == 0)
                return null;

            return branchDetails
                .Select(BranchMapper.ToBranchDetailViewModel)
                .ToList();
        }

        public async Task<CartPriceDetailViewModel?> GetProductDetailByIdAsync(int productDetailId, int? branchId, string? role)
        {
            var isGuestOrCustomer = string.IsNullOrEmpty(role) || role == "Customer";
            int? resolvedBranchId = branchId;

            if (resolvedBranchId <= 0)
                resolvedBranchId = null;

            if (resolvedBranchId == null)
            {
                if (isGuestOrCustomer)
                {
                    var onlineBranch = await _context.Branches
                        .FirstOrDefaultAsync(b => b.IsOnline);

                    if (onlineBranch == null)
                        return null;

                    resolvedBranchId = onlineBranch.BranchId;
                }
                else
                {
                    return null;
                }
            }
            var branchDetail = await GetBranchDetailEntityAsync(productDetailId, resolvedBranchId.Value);
            if (branchDetail == null) return null;

            return BranchMapper.ToCartPriceDetailViewModel(branchDetail);
        }

        private async Task<BranchDetail?> GetBranchDetailEntityAsync(int productDetailId, int branchId)
        {
            return await _context.BranchDetails
                .Include(x => x.ProductDetail)
                    .ThenInclude(pd => pd.Product)
                .Include(x => x.ProductDetail.Color)
                .Include(x => x.ProductDetail.Size)
                .Include(x => x.ProductDetail.Fabric)
                .Include(x => x.ProductDetail.Images)
                .Include(x => x.ProductDetail.DiscountPriceDetails)
                    .ThenInclude(dp => dp.DiscountPrice)
                .Where(x => x.ProductDetail.DetailId == productDetailId)
                .Where(x => x.BranchId == branchId)
                .FirstOrDefaultAsync();
        }

        public async Task<List<ProductViewModel>> SmartSearchProductsAsync(int branchId, string nameLike, int? idMatch)
        {
            var query = _context.BranchDetails
                .Include(x => x.ProductDetail)
                    .ThenInclude(pd => pd.Product)
                .Include(x => x.ProductDetail.Images)
                .Where(x => x.BranchId == branchId);

            if (!string.IsNullOrEmpty(nameLike))
            {
                query = query.Where(x => x.ProductDetail.Product.ProductName.Contains(nameLike));
            }

            if (idMatch.HasValue)
            {
                query = query.Where(x => x.ProductDetail.Product.ProductId == idMatch.Value);
            }

            var products = await query
                .Select(x => new
                {
                    Product = x.ProductDetail.Product,
                    BranchDetail = x
                })
                .GroupBy(p => p.Product.ProductId)
                .Select(g => g.First())
                .ToListAsync();

            return products
                .Select(p => ProductMapper.ToBranchProductViewModel(p.Product, p.BranchDetail))
                .ToList();
        }

        public async Task<bool> DeleteBranchAsync(int branchId)
        {
            var branch = await _context.Branches
                .Include(b => b.Employees)
                .Include(b => b.BranchDetails)
                .FirstOrDefaultAsync(b => b.BranchId == branchId);

            if (branch == null)
                return false;

            if (branch.Employees.Any() || branch.BranchDetails.Any())
                throw new InvalidOperationException("Chi nhánh đã có nhân viên hoặc sản phẩm, không thể xóa.");

            _context.Branches.Remove(branch);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
