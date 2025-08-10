using MenShop_Assignment.Datas;
using MenShop_Assignment.DTOs;
using MenShop_Assignment.Models;
using MenShop_Assignment.Models.ProductModels.ReponseDTO;

namespace MenShop_Assignment.Repositories.CollectionRepository
{
    public interface ICollectionRepository
    {
        Task<ApiResponseModel<List<CollectionViewModel>>> GetAllCollection();
        Task<ApiResponseModel<CollectionViewModel?>> GetByIdCollection(int collectionId);
        Task<ApiResponseModel<bool>> AddCollection(Collection collection);
        Task<ApiResponseModel<bool>> UpdateCollection(Collection updatedCollection);
        Task<ApiResponseModel<bool>> DeleteCollection(int collectionId);
        Task<ApiResponseModel<List<CollectionDetailsViewModel>>> GetCollectionDetailsByCollectionId(int collectionId);
        Task<ApiResponseModel<bool>> AddDetail(CollectionDetail detail);
        Task<ApiResponseModel<bool>> UpdateDetail(CollectionDetail detail);
        Task<ApiResponseModel<bool>> DeleteDetail(int detailId);
        Task<ApiResponseModel<bool>> UpdateCollectionStatus(int collectionId);
        Task<List<CreateImageResponse>> AddImagesToCollectionAsync(int collectionId, List<string> imageUrls);
        Task<List<ImageCollectionViewModel>> GetImgByCollectionIdAsync(int collectionId);
        Task DeleteImageAsync(int imageId);
    }

}
