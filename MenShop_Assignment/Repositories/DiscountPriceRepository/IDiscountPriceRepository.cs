
using MenShop_Assignment.Datas;
using MenShop_Assignment.DTOs;
using MenShop_Assignment.Mapper;
using MenShop_Assignment.Models;

namespace MenShop_Assignment.Repositories.DiscountPriceRepository
{
    public interface IDiscountPriceRepository
    {
        Task<ApiResponseModel<bool>> CreateDiscount(DiscountPrice discountPrice);
        Task<ApiResponseModel<bool>> CreateDiscountDetail(DiscountPriceDetail discountPriceDetail);
        Task<ApiResponseModel<bool>> DeleteDiscount(int Id);
        Task<ApiResponseModel<bool>> DeleteDiscountDetail(int Id);
        Task<ApiResponseModel<List<DiscountPriceViewModel>>> GetAllDiscountPrice();
        Task<ApiResponseModel<DiscountPriceDetailViewModel?>> GetByIdDiscountDetailPrice(int Id);
        Task<ApiResponseModel<DiscountPriceViewModel?>> GetByIdDiscountPrice(int Id);
        Task<ApiResponseModel<List<DiscountPriceDetailViewModel>>> GetDiscountDetailsByProductDetailId(int productDetailId);
        Task<ApiResponseModel<List<DiscountPriceDetailViewModel>>> GetProductDetailsByDiscountId(int discountPriceId);
        Task<ApiResponseModel<bool>> UpdateDiscount(DiscountPrice discountPrice);
        Task<ApiResponseModel<bool>> UpdateDiscountDetail(DiscountPriceDetail discountPrice);
    }
}
