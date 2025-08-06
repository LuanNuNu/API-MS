using MenShop_Assignment.Datas;
using MenShop_Assignment.DTOs;
using MenShop_Assignment.Mapper;
using MenShop_Assignment.Models;
using Microsoft.EntityFrameworkCore;

namespace MenShop_Assignment.Repositories.DiscountPriceRepository
{
    public class DiscountPriceRepository : IDiscountPriceRepository
    {
        private readonly ApplicationDbContext _context;

        public DiscountPriceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponseModel<List<DiscountPriceViewModel>>> GetAllDiscountPrice()
        {
            var discountPrices = await _context.DiscountPrices.ToListAsync();
            var result = discountPrices.Select(DiscountPriceMapper.ToDiscountPriceViewModel).ToList();

            return new ApiResponseModel<List<DiscountPriceViewModel>>(true, "Lấy danh sách thành công", result, 200);
        }

        public async Task<ApiResponseModel<DiscountPriceViewModel?>> GetByIdDiscountPrice(int Id)
        {
            var discountPrice = await _context.DiscountPrices.FirstOrDefaultAsync(x => x.Id == Id);
            if (discountPrice == null)
                return new ApiResponseModel<DiscountPriceViewModel?>(false, "Không tìm thấy khuyến mãi", null, 404);

            var viewModel = DiscountPriceMapper.ToDiscountPriceViewModel(discountPrice);
            return new ApiResponseModel<DiscountPriceViewModel?>(true, "Thành công", viewModel, 200);
        }

        public async Task<ApiResponseModel<bool>> CreateDiscount(DiscountPrice discountPrice)
        {
            try
            {
                _context.DiscountPrices.Add(discountPrice);
                await _context.SaveChangesAsync();
                return new ApiResponseModel<bool>(true, "Tạo khuyến mãi thành công", true, 201);
            }
            catch (Exception ex)
            {
                return new ApiResponseModel<bool>(false, "Lỗi khi tạo khuyến mãi", false, 500, new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponseModel<bool>> UpdateDiscount(DiscountPrice discountPrice)
        {
            try
            {
                var existing = await _context.DiscountPrices.FindAsync(discountPrice.Id);
                if (existing == null)
                    return new ApiResponseModel<bool>(false, "Không tìm thấy khuyến mãi", false, 404);

                existing.Name = discountPrice.Name;
                existing.DiscountPercent = discountPrice.DiscountPercent;
                existing.StartTime = discountPrice.StartTime;
                existing.EndTime = discountPrice.EndTime;

                _context.DiscountPrices.Update(existing);
                await _context.SaveChangesAsync();
                return new ApiResponseModel<bool>(true, "Cập nhật thành công", true, 200);
            }
            catch (Exception ex)
            {
                return new ApiResponseModel<bool>(false, "Lỗi khi cập nhật", false, 500, new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponseModel<bool>> DeleteDiscount(int Id)
        {
            try
            {
                var discountPrice = await _context.DiscountPrices
                    .Include(c => c.discountPriceDetails)
                    .FirstOrDefaultAsync(c => c.Id == Id);

                if (discountPrice == null)
                    return new ApiResponseModel<bool>(false, "Không tìm thấy khuyến mãi", false, 404);

                if (discountPrice.discountPriceDetails != null && discountPrice.discountPriceDetails.Any())
                    return new ApiResponseModel<bool>(false, "Không thể xoá vì có sản phẩm liên quan", false, 400);

                _context.DiscountPrices.Remove(discountPrice);
                await _context.SaveChangesAsync();
                return new ApiResponseModel<bool>(true, "Xoá thành công", true, 200);
            }
            catch (Exception ex)
            {
                return new ApiResponseModel<bool>(false, "Lỗi khi xoá", false, 500, new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponseModel<DiscountPriceDetailViewModel?>> GetByIdDiscountDetailPrice(int Id)
        {
            var detail = await _context.DiscountPriceDetails.FirstOrDefaultAsync(x => x.Id == Id);
            if (detail == null)
                return new ApiResponseModel<DiscountPriceDetailViewModel?>(false, "Không tìm thấy chi tiết", null, 404);

            var result = DiscountPriceMapper.ToDiscountDetailById(detail);
            return new ApiResponseModel<DiscountPriceDetailViewModel?>(true, "Thành công", result, 200);
        }

        public async Task<ApiResponseModel<List<DiscountPriceDetailViewModel>>> GetProductDetailsByDiscountId(int discountPriceId)
        {
            var discountDetails = await _context.DiscountPriceDetails
                .Where(d => d.discountPriceId == discountPriceId)
                .Include(d => d.ProductDetail).ThenInclude(p => p.Product)
                .Include(d => d.ProductDetail).ThenInclude(p => p.Color)
                .Include(d => d.ProductDetail).ThenInclude(p => p.Size)
                .Include(d => d.ProductDetail).ThenInclude(p => p.Fabric)
                .ToListAsync();

            var result = discountDetails.Select(DiscountPriceMapper.ToDiscountById).ToList();
            return new ApiResponseModel<List<DiscountPriceDetailViewModel>>(true, "Thành công", result, 200);
        }

        public async Task<ApiResponseModel<bool>> CreateDiscountDetail(DiscountPriceDetail discountPriceDetail)
        {
            try
            {
                bool isExisted = await _context.DiscountPriceDetails
                    .AnyAsync(x => x.productDetailId == discountPriceDetail.productDetailId);

                if (isExisted)
                    return new ApiResponseModel<bool>(false, "Sản phẩm đã có khuyến mãi", false, 400);

                _context.DiscountPriceDetails.Add(discountPriceDetail);
                await _context.SaveChangesAsync();
                return new ApiResponseModel<bool>(true, "Thêm thành công", true, 201);
            }
            catch (Exception ex)
            {
                return new ApiResponseModel<bool>(false, "Lỗi khi thêm", false, 500, new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponseModel<bool>> UpdateDiscountDetail(DiscountPriceDetail discountPrice)
        {
            try
            {
                var existing = await _context.DiscountPriceDetails.FindAsync(discountPrice.Id);
                if (existing == null)
                    return new ApiResponseModel<bool>(false, "Không tìm thấy chi tiết", false, 404);

                existing.discountPriceId = discountPrice.discountPriceId;
                existing.productDetailId = discountPrice.productDetailId;

                _context.DiscountPriceDetails.Update(existing);
                await _context.SaveChangesAsync();
                return new ApiResponseModel<bool>(true, "Cập nhật thành công", true, 200);
            }
            catch (Exception ex)
            {
                return new ApiResponseModel<bool>(false, "Lỗi khi cập nhật", false, 500, new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponseModel<bool>> DeleteDiscountDetail(int Id)
        {
            try
            {
                var discountPrice = await _context.DiscountPriceDetails.FindAsync(Id);
                if (discountPrice == null)
                    return new ApiResponseModel<bool>(false, "Không tìm thấy chi tiết", false, 404);

                _context.DiscountPriceDetails.Remove(discountPrice);
                await _context.SaveChangesAsync();
                return new ApiResponseModel<bool>(true, "Xoá thành công", true, 200);
            }
            catch (Exception ex)
            {
                return new ApiResponseModel<bool>(false, "Lỗi khi xoá", false, 500, new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponseModel<List<DiscountPriceDetailViewModel>>> GetDiscountDetailsByProductDetailId(int productDetailId)
        {
            var details = await _context.DiscountPriceDetails
                .Where(d => d.productDetailId == productDetailId)
                .Include(d => d.DiscountPrice)
                .Include(d => d.ProductDetail).ThenInclude(p => p.Product)
                .Include(d => d.ProductDetail).ThenInclude(p => p.Color)
                .Include(d => d.ProductDetail).ThenInclude(p => p.Size)
                .Include(d => d.ProductDetail).ThenInclude(p => p.Fabric)
                .ToListAsync();

            var result = details.Select(DiscountPriceMapper.ToDiscountById).ToList();
            return new ApiResponseModel<List<DiscountPriceDetailViewModel>>(true, "Thành công", result, 200);
        }
    }

}