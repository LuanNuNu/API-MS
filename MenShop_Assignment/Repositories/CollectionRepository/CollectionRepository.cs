using MenShop_Assignment.Datas;
using MenShop_Assignment.Mapper;
using MenShop_Assignment.Models;
using Microsoft.EntityFrameworkCore;

namespace MenShop_Assignment.Repositories.CollectionRepository
{
    public class CollectionRepository : ICollectionRepository
    {
        private readonly ApplicationDbContext _context;

        public CollectionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponseModel<List<CollectionViewModel>>> GetAllCollection()
        {
            var collections = await _context.Collections.ToListAsync();
            var result = collections.Select(CollectionMapper.ToCollectionViewModel).ToList();

            return new ApiResponseModel<List<CollectionViewModel>>(true, "Lấy danh sách thành công", result, 200);
        }

        public async Task<ApiResponseModel<CollectionViewModel?>> GetByIdCollection(int collectionId)
        {
            var collection = await _context.Collections.FirstOrDefaultAsync(c => c.CollectionId == collectionId);
            if (collection == null)
            {
                return new ApiResponseModel<CollectionViewModel?>(false, "Không tìm thấy bộ sưu tập", null, 404);
            }

            return new ApiResponseModel<CollectionViewModel?>(true, "Thành công", CollectionMapper.ToCollectionViewModel(collection), 200);
        }

        public async Task<ApiResponseModel<bool>> AddCollection(Collection collection)
        {
            try
            {
                _context.Collections.Add(collection);
                await _context.SaveChangesAsync();
                return new ApiResponseModel<bool>(true, "Thêm thành công", true, 201);
            }
            catch (DbUpdateException ex)
            {
                return new ApiResponseModel<bool>(false, "Lỗi khi thêm bộ sưu tập", false, 500, new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponseModel<bool>> UpdateCollection(Collection updatedCollection)
        {
            try
            {
                var existing = await _context.Collections.FindAsync(updatedCollection.CollectionId);
                if (existing == null)
                    return new ApiResponseModel<bool>(false, "Không tìm thấy bộ sưu tập", false, 404);

                existing.CollectionName = updatedCollection.CollectionName;
                existing.Description = updatedCollection.Description;
                existing.StartTime = updatedCollection.StartTime;
                existing.EndTime = updatedCollection.EndTime;
                existing.Status = updatedCollection.Status;

                _context.Collections.Update(existing);
                await _context.SaveChangesAsync();
                return new ApiResponseModel<bool>(true, "Cập nhật thành công", true, 200);
            }
            catch (DbUpdateException ex)
            {
                return new ApiResponseModel<bool>(false, "Lỗi khi cập nhật", false, 500, new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponseModel<bool>> DeleteCollection(int collectionId)
        {
            try
            {
                var collection = await _context.Collections
                    .Include(c => c.CollectionDetails)
                    .Include(c => c.Images)
                    .FirstOrDefaultAsync(c => c.CollectionId == collectionId);

                if (collection == null)
                    return new ApiResponseModel<bool>(false, "Không tìm thấy bộ sưu tập", false, 404);

                if (collection.CollectionDetails != null && collection.CollectionDetails.Any())
                    return new ApiResponseModel<bool>(false, "Không thể xoá bộ sưu tập có sản phẩm liên quan", false, 400);

                _context.Collections.Remove(collection);
                await _context.SaveChangesAsync();

                return new ApiResponseModel<bool>(true, "Xoá thành công", true, 200);
            }
            catch (DbUpdateException ex)
            {
                return new ApiResponseModel<bool>(false, "Lỗi khi xoá", false, 500, new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponseModel<List<CollectionDetailsViewModel>>> GetCollectionDetailsByCollectionId(int collectionId)
        {
            var details = await _context.CollectionDetails
                .Include(cd => cd.Product)
                    .ThenInclude(p => p.ProductDetails)
                        .ThenInclude(pd => pd.Color)
                .Include(cd => cd.Product)
                    .ThenInclude(p => p.ProductDetails)
                        .ThenInclude(pd => pd.Size)
                .Include(cd => cd.Product)
                    .ThenInclude(p => p.ProductDetails)
                        .ThenInclude(pd => pd.Fabric)
                .Where(cd => cd.CollectionId == collectionId)
                .ToListAsync();

            var result = details.Select(CollectionMapper.ToProductColectionViewModel).ToList();
            return new ApiResponseModel<List<CollectionDetailsViewModel>>(true, "Lấy dữ liệu hành công", result, 200);
        }

        public async Task<ApiResponseModel<bool>> AddDetail(CollectionDetail detail)
        {
            try
            {
                bool exists = await _context.CollectionDetails
                    .AnyAsync(cd => cd.CollectionId == detail.CollectionId && cd.ProductId == detail.ProductId);

                if (exists)
                    return new ApiResponseModel<bool>(false, "Sản phẩm đã tồn tại trong bộ sưu tập", false, 400);

                _context.CollectionDetails.Add(detail);
                await _context.SaveChangesAsync();
                return new ApiResponseModel<bool>(true, "Thêm sản phẩm vào bộ sưu tập thành công", true, 201);
            }
            catch (DbUpdateException ex)
            {
                return new ApiResponseModel<bool>(false, "Lỗi khi thêm sản phẩm", false, 500, new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponseModel<bool>> UpdateDetail(CollectionDetail detail)
        {
            try
            {
                var existing = await _context.CollectionDetails.FindAsync(detail.CollectionDetailId);
                if (existing == null)
                    return new ApiResponseModel<bool>(false, "Không tìm thấy chi tiết bộ sưu tập", false, 404);

                existing.ProductId = detail.ProductId;
                existing.CollectionId = detail.CollectionId;

                _context.CollectionDetails.Update(existing);
                await _context.SaveChangesAsync();
                return new ApiResponseModel<bool>(true, "Cập nhật thành công", true, 200);
            }
            catch (DbUpdateException ex)
            {
                return new ApiResponseModel<bool>(false, "Lỗi khi cập nhật", false, 500, new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponseModel<bool>> DeleteDetail(int detailId)
        {
            try
            {
                var detail = await _context.CollectionDetails.FindAsync(detailId);
                if (detail == null)
                    return new ApiResponseModel<bool>(false, "Không tìm thấy chi tiết", false, 404);

                _context.CollectionDetails.Remove(detail);
                await _context.SaveChangesAsync();
                return new ApiResponseModel<bool>(true, "Xoá chi tiết thành công", true, 200);
            }
            catch (DbUpdateException ex)
            {
                return new ApiResponseModel<bool>(false, "Lỗi khi xoá", false, 500, new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponseModel<bool>> UpdateCollectionStatus(int collectionId, bool newStatus)
        {
            try
            {
                var collection = await _context.Collections.FindAsync(collectionId);
                if (collection == null)
                    return new ApiResponseModel<bool>(false, "Không tìm thấy bộ sưu tập", false, 404);

                collection.Status = newStatus;
                _context.Collections.Update(collection);
                await _context.SaveChangesAsync();
                return new ApiResponseModel<bool>(true, "Cập nhật trạng thái thành công", true, 200);
            }
            catch (DbUpdateException ex)
            {
                return new ApiResponseModel<bool>(false, "Lỗi khi cập nhật trạng thái", false, 500, new List<string> { ex.Message });
            }
        }
    }

}