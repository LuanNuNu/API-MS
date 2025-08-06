using MenShop_Assignment.Datas;
using MenShop_Assignment.Models;

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
        Task<ApiResponseModel<bool>> UpdateCollectionStatus(int collectionId, bool newStatus);
    }

}
