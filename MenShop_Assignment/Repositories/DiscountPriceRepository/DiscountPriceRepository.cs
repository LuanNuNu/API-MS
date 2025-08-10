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

        public async Task<ApiResponseModel<bool>> CreateDiscount(CreateDiscountPriceDTO dto)
        {
            try
            {
                var discountPrice = new DiscountPrice
                {
                    Name = dto.Name,
                    DiscountPercent = dto.DiscountPercent,
                    Description = dto.Description,
                    StartTime = dto.StartTime,
                    EndTime = dto.EndTime,
                    IsActive = dto.IsActive
                };

                _context.DiscountPrices.Add(discountPrice);
                await _context.SaveChangesAsync();

                return new ApiResponseModel<bool>(true, "Tạo chương trình khuyến mãi thành công", true, 201);
            }
            catch (Exception ex)
            {
                return new ApiResponseModel<bool>(false, "Lỗi khi tạo khuyến mãi", false, 500, new List<string> { ex.Message });
            }
        }


        public async Task<ApiResponseModel<bool>> UpdateDiscount(int id, CreateDiscountPriceDTO dto)
        {
            try
            {
                var existing = await _context.DiscountPrices.FindAsync(id);
                if (existing == null)
                    return new ApiResponseModel<bool>(false, "Không tìm thấy khuyến mãi", false, 404);

                existing.Name = dto.Name;
                existing.Description = dto.Description;
                existing.DiscountPercent = dto.DiscountPercent;
                existing.StartTime = dto.StartTime;
                existing.EndTime = dto.EndTime;
                existing.IsActive = dto.IsActive;

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

        public async Task<ApiResponseModel<object>> CreateDiscountDetails(CreateDiscountDetailDTO dto)
        {
            var failedIds = new List<int>();

            try
            {
                foreach (var productDetailId in dto.productDetailIds)
                {
                    bool isExisted = await _context.DiscountPriceDetails
                        .AnyAsync(x => x.productDetailId == productDetailId);

                    if (isExisted)
                    {
                        failedIds.Add(productDetailId);
                        continue;
                    }

                    var detail = new DiscountPriceDetail
                    {
                        discountPriceId = dto.discountPriceId,
                        productDetailId = productDetailId
                    };

                    _context.DiscountPriceDetails.Add(detail);
                }

                await _context.SaveChangesAsync();

                if (failedIds.Any())
                {
                    return new ApiResponseModel<object>(
                        false,
                        "Một số sản phẩm đã có chương trình khuyến mãi hoặc lỗi khi thêm.",
                        false,
                        207,
                        new List<string>()
                    );
                }

                return new ApiResponseModel<object>(
                    true,
                    "Tất cả sản phẩm đã được thêm vào khuyến mãi.",
                    true,
                    201
                );
            }
            catch (Exception ex)
            {
                return new ApiResponseModel<object>(
                    false,
                    "Lỗi khi thêm chi tiết khuyến mãi.",
                    false,
                    500,
                    new List<string> { ex.Message }
                );
            }
        }


        public async Task<ApiResponseModel<bool>> UpdateDiscountDetail(int id, UpdateDiscountDetailDTO dto)
        {
            try
            {
                var existing = await _context.DiscountPriceDetails.FindAsync(id);
                if (existing == null)
                    return new ApiResponseModel<bool>(false, "Không tìm thấy chi tiết", false, 404);

                // Mapping
                existing.discountPriceId = dto.discountPriceId;
                existing.productDetailId = dto.productDetailId;

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
        public async Task<ApiResponseModel<DiscountPriceDetailViewModel?>> GetDiscountDetailsByProductDetailId(int productDetailId)
        {
            var detail = await _context.DiscountPriceDetails
                .Where(d => d.productDetailId == productDetailId)
                .Include(d => d.DiscountPrice)
                .Include(d => d.ProductDetail).ThenInclude(p => p.Product)
                .Include(d => d.ProductDetail).ThenInclude(p => p.Color)
                .Include(d => d.ProductDetail).ThenInclude(p => p.Size)
                .Include(d => d.ProductDetail).ThenInclude(p => p.Fabric)
                .FirstOrDefaultAsync();

            if (detail == null)
            {
                return new ApiResponseModel<DiscountPriceDetailViewModel?>(
                    false, "Không tìm thấy giảm giá cho sản phẩm này", null, 404
                );
            }

            var result = DiscountPriceMapper.ToDiscountById(detail);
            return new ApiResponseModel<DiscountPriceDetailViewModel?>(
                true, "Thành công", result, 200
            );
        }


        public async Task<bool> UpdateDiscountStatusAsync(int discountId)
        {
            var discount = await _context.DiscountPrices.FindAsync(discountId);
            if (discount == null)
            {
                return false;
            }

            discount.IsActive = discount.IsActive == true ? false : true;
            _context.DiscountPrices.Update(discount);
            await _context.SaveChangesAsync();

            return true;
        }
    }

}