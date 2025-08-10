
using MenShop_Assignment.Datas;
using MenShop_Assignment.DTOs;
using MenShop_Assignment.Mapper;
using MenShop_Assignment.Models;

namespace MenShop_Assignment.Repositories.DiscountPriceRepository
{
    public interface IDiscountPriceRepository
    {
        Task<ApiResponseModel<bool>> CreateDiscount(CreateDiscountPriceDTO dto);
        Task<ApiResponseModel<object>> CreateDiscountDetails(CreateDiscountDetailDTO dto);
        Task<ApiResponseModel<bool>> DeleteDiscount(int Id);
        Task<ApiResponseModel<bool>> DeleteDiscountDetail(int Id);
        Task<ApiResponseModel<List<DiscountPriceViewModel>>> GetAllDiscountPrice();
        Task<ApiResponseModel<DiscountPriceDetailViewModel?>> GetByIdDiscountDetailPrice(int Id);
        Task<ApiResponseModel<DiscountPriceViewModel?>> GetByIdDiscountPrice(int Id);

        Task<ApiResponseModel<DiscountPriceDetailViewModel>> GetDiscountDetailsByProductDetailId(int productDetailId);
        Task<ApiResponseModel<List<DiscountPriceDetailViewModel>>> GetProductDetailsByDiscountId(int discountPriceId);
        Task<ApiResponseModel<bool>> UpdateDiscount(int id, CreateDiscountPriceDTO dto);
        Task<ApiResponseModel<bool>> UpdateDiscountDetail(int id, UpdateDiscountDetailDTO dto);
        Task<bool> UpdateDiscountStatusAsync(int discountId);
    }
}
