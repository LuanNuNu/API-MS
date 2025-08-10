using MenShop_Assignment.Datas;
using MenShop_Assignment.DTOs;
using MenShop_Assignment.Mapper;
using MenShop_Assignment.Models;
using MenShop_Assignment.Models.ProductModels.ReponseDTO;
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
            var collections = await _context.Collections          
                .Where(c => c.Status == true)
                .ToListAsync();
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

        public async Task<ApiResponseModel<bool>> UpdateCollectionStatus(int collectionId)
        {
            try
            {
                var collection = await _context.Collections.FindAsync(collectionId);
                if (collection == null)
                    return new ApiResponseModel<bool>(false, "Không tìm thấy bộ sưu tập", false, 404);

    
                collection.Status = collection.Status == true ? false : true;
                _context.Collections.Update(collection);

                var products = await (
                     from p in _context.Products
                     join cd in _context.CollectionDetails
                         on p.ProductId equals cd.ProductId
                     where cd.CollectionId == collectionId
                     select p
                 ).ToListAsync();



                foreach (var product in products)
                {
                    product.Status = collection.Status; 
                }

                _context.Products.UpdateRange(products);
                await _context.SaveChangesAsync();

                return new ApiResponseModel<bool>(true, "Cập nhật trạng thái thành công", true, 200);
            }
            catch (DbUpdateException ex)
            {
                return new ApiResponseModel<bool>(false, "Lỗi khi cập nhật trạng thái", false, 500, new List<string> { ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }


        public async Task<List<CreateImageResponse>> AddImagesToCollectionAsync(int collectionId, List<string> imageUrls)
        {
            var responses = new List<CreateImageResponse>();

            try
            {
                foreach (var url in imageUrls)
                {
                    var path = Path.GetFileName(new Uri(url).LocalPath);
                    bool isDuplicate = await _context.ImageCollections
                        .AnyAsync(i => i.CollectionId == collectionId && i.Path == path);

                    if (isDuplicate)
                    {
                        responses.Add(new CreateImageResponse
                        {
                            IsSuccess = false,
                            Message = $"Ảnh với tên `{path}` đã tồn tại."
                        });
                        continue;
                    }

                    var image = new ImageCollection
                    {
                        Path = path,
                        FullPath = url,
                        CollectionId = collectionId
                    };

                    _context.ImageCollections.Add(image);
                    await _context.SaveChangesAsync();

                    var res = CollectionMapper.ToCreateImageResponse(image);
                    res.IsSuccess = true;
                    res.Message = $"Thêm ảnh `{path}` thành công.";
                    responses.Add(res);
                }
            }
            catch (Exception ex)
            {
                responses.Add(new CreateImageResponse
                {
                    IsSuccess = false,
                    Message = "Lỗi: " + ex.Message
                });
            }

            return responses;
        }

        public async Task DeleteImageAsync(int imageId)
        {
            var image = await _context.ImageCollections.FindAsync(imageId);
            if (image == null)
            {
                throw new Exception($"Ảnh với ID {imageId} không tồn tại.");
            }
            _context.ImageCollections.Remove(image);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ImageCollectionViewModel>> GetImgByCollectionIdAsync(int collectionId)
        {
            var images = await _context.ImageCollections
                .Where(dt => dt.CollectionId == collectionId)
                .ToListAsync();

            return images.Select(CollectionMapper.ToImageViewModel).ToList();
        }
    }

}