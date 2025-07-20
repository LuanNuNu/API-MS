using MenShop_Assignment.Datas;
using MenShop_Assignment.DTOs;
using MenShop_Assignment.Mapper;
using MenShop_Assignment.Models;
using MenShop_Assignment.Models.ProductModels.ReponseDTO;
using MenShop_Assignment.Repositories.Product;
using Microsoft.EntityFrameworkCore;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;


    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

	public async Task<IEnumerable<ProductViewModel>> GetAllProductsAsync()
	{
		var products = await _context.Products
			.Include(p => p.Category)
			.AsNoTracking()
			.ToListAsync();

		return products.Select(ProductMapper.ToProductViewModel).ToList();
	}
	public async Task<List<ProductDetailViewModel>> GetProductDetailsByProductIdAsync(int productId)
	{
		var details = await _context.ProductDetails
			.Where(d => d.ProductId == productId)
			.Include(d => d.Size)
			.Include(d => d.Color)
			.Include(d => d.Fabric)
			.Include(d => d.HistoryPrices)
			.Include(d => d.Images)
			.AsSplitQuery()
			.AsNoTracking()
			.ToListAsync();

		return details.Select(ProductMapper.ToProductDetailViewModel).ToList();
	}
	//up1
	public async Task<List<ImageProductViewModel>> GetImgByProductDetailIdAsync(int productDetailId)
	{
		var images = await _context.ImagesProducts
			.Where(dt => dt.ProductDetailId == productDetailId)
			.ToListAsync();

		return images.Select(ProductMapper.ToImageProductViewModel).ToList();
	}
	public async Task<ProductViewModel?> GetProductByIdAsync(int productId)
	{
		var product = await _context.Products
			.FirstOrDefaultAsync(p => p.ProductId == productId);

		if (product == null)
		{
			return null;
		}

		return ProductMapper.ToProductViewModel(product);
	}

    public async Task<List<ProductViewModel>> GetProductsByCategoryIdAsync(int categoryId)
    {
        var products = await _context.Products
            .Include(p => p.Category)
            .AsNoTracking()
            .Where(p => p.CategoryId == categoryId)
            .ToListAsync();

        return products.Select(ProductMapper.ToProductViewModel).ToList();
    }



    public async Task<ProductResponseDTO> UpdateProductAsync(int productId, UpdateProductDTO dto)
	{
		var response = new ProductResponseDTO();
		try
		{
			await ValidateCategoryExistsAsync(dto.CategoryId);
			var product = await _context.Products.FindAsync(productId);

			if (product == null)
				return new ProductResponseDTO { IsSuccess = false, Message = "Không tìm thấy sản phẩm!" };
            bool exists = await _context.Products.AnyAsync(p =>
           p.ProductName == dto.ProductName &&
           p.CategoryId == dto.CategoryId &&
           p.ProductId != productId);

            if (exists)
                return new ProductResponseDTO { IsSuccess = false, Message = "Tên sản phẩm đã tồn tại trong danh mục này!" };
            product.ProductName = dto.ProductName;
			product.Description = dto.Description;
			product.Status = dto.Status;
			product.CategoryId = dto.CategoryId;

			await _context.SaveChangesAsync();

			response = ProductMapper.ToCreateAndUpdateProductResponse(product);
			response.IsSuccess = true;
			response.Message = "Cập nhật thông tin sản phẩm thành công";
		}
		catch (Exception ex)
		{
			response.IsSuccess = false;
			response.Message = ex.Message;
		}

		return response;
	}
	public async Task<ProductDetailResponse> UpdateProductDetailsAsync(UpdateProductDetailDTO details)
	{
		var response = new ProductDetailResponse();
		try
		{
			await ValidateProductDetailReferencesAsync(details.SizeId, details.ColorId, details.FabricId);
			var detail = await _context.ProductDetails.FindAsync(details.DetailId);
			if (detail == null)
			{
				response.Message = "Không tìm thấy chi tiết sản phẩm.";
				return response;
			}
            bool exists = await _context.ProductDetails.AnyAsync(d =>
				d.ProductId == detail.ProductId &&
				d.SizeId == details.SizeId &&
				d.ColorId == details.ColorId &&
				d.FabricId == details.FabricId &&
				d.DetailId != details.DetailId);

            if (exists)
            {
                response.IsSuccess = false;
                response.Message = "Chi tiết sản phẩm này đã tồn tại.";
                return response;
            }
            detail.SizeId = details.SizeId;
			detail.ColorId = details.ColorId;
			detail.FabricId = details.FabricId;

			await _context.SaveChangesAsync();

			response.IsSuccess = true;
			response.Message = "Cập nhật chi tiết thành công.";
		}
		catch (Exception ex)
		{
			response.IsSuccess = false;
			response.Message = ex.Message;
		}

		return response;
	}

	public async Task<ImageResponse> UpdateProductDetailImagesAsync(int detailId, List<UpdateImageDTO> images)
	{
		var response = new ImageResponse();
		try
		{

			var detail = await _context.ProductDetails
				.Include(d => d.Images)
				.FirstOrDefaultAsync(d => d.DetailId == detailId);

			if (detail == null)
			{
				response.Message = "Không tìm thấy chi tiết sản phẩm.";
				return response;
			}

			foreach (var imgDto in images)
			{
				if (imgDto.ImageId > 0)
				{
					await ValidateImageExistsAsync(imgDto.ImageId, detailId);
					var img = detail.Images.FirstOrDefault(i => i.Id == imgDto.ImageId);
                    bool duplicate = detail.Images.Any(i =>
						i.FullPath == imgDto.ImageUrl &&
						i.Id != imgDto.ImageId);

                    if (duplicate)
                    {
                        response.IsSuccess = false;
                        response.Message = $"Ảnh `{imgDto.ImageUrl}` đã tồn tại ở chi tiết này.";
                        return response;
                    }
                    if (img != null && !string.Equals(img.FullPath, imgDto.ImageUrl, StringComparison.OrdinalIgnoreCase))
					{
						img.FullPath = imgDto.ImageUrl;
					}
				}
			}
			await _context.SaveChangesAsync();

			response.IsSuccess = true;
			response.Message = "Cập nhật ảnh thành công.";
		}
		catch (Exception ex)
		{
			response.Message = ex.Message;
		}

		return response;
	}



	public async Task<bool> UpdateProductStatusAsync(int productId)
	{
		var product = await _context.Products.FindAsync(productId);
		if (product == null)
		{
			return false;
		}

		product.Status = product.Status == true ? false : true;
		_context.Products.Update(product);
		await _context.SaveChangesAsync();

		return true;
	}
    ///tách nhỏ
	///
    public async Task<ProductResponseDTO> CreateProductOnlyAsync(CreateProductDTO dto)
    {
        var response = new ProductResponseDTO();
        try
        {
            await ValidateCategoryExistsAsync(dto.CategoryId);

            var exists = await _context.Products.AnyAsync(p =>
                p.ProductName.ToLower() == dto.ProductName.Trim().ToLower()
                && p.CategoryId == dto.CategoryId);

            if (exists)
            {
                response.IsSuccess = false;
                response.Message = "Sản phẩm đã tồn tại trong danh mục.";
                return response;
            }

            var product = new Product
            {
                ProductName = dto.ProductName,
                Description = dto.Description,
                CategoryId = dto.CategoryId,
                Status = dto.Status,
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            response = ProductMapper.ToCreateAndUpdateProductResponse(product);
            response.IsSuccess = true;
            response.Message = "Tạo sản phẩm thành công.";
        }
        catch (Exception ex)
        {
            response.IsSuccess = false;
            response.Message = "Lỗi: " + ex.Message;
        }

        return response;
    }

    //up2
    public async Task<CreateProductDetailResponse> AddProductDetailsAsync(AddProductDetailDTO dto)
    {
        var response = new CreateProductDetailResponse();

        var productExists = await _context.Products.AnyAsync(p => p.ProductId == dto.ProductId);
        if (!productExists)
        {
            response.Results.Add(new ProductDetailResult
            {
                IsSuccess = false,
                Message = $"Sản phẩm với ID {dto.ProductId} không tồn tại"
            });
            return response;
        }

        if (dto.Details == null || dto.Details.Count == 0)
        {
            response.Results.Add(new ProductDetailResult
            {
                IsSuccess = false,
                Message = "Không có chi tiết nào được cung cấp."
            });
            return response;
        }

        foreach (var item in dto.Details)
        {
            var result = new ProductDetailResult
            {
                Detail = new ProductDetailDTO
                {
                    ProductId = dto.ProductId,
                    SizeId = item.SizeId,
                    ColorId = item.ColorId,
                    FabricId = item.FabricId
                }
            };

            try
            {
                await ValidateProductDetailReferencesAsync(item.SizeId, item.ColorId, item.FabricId);
                var isDuplicate = await _context.ProductDetails.AnyAsync(pd =>
                    pd.ProductId == dto.ProductId &&
                    pd.SizeId == item.SizeId &&
                    pd.ColorId == item.ColorId &&
                    pd.FabricId == item.FabricId);

                if (isDuplicate)
                {
                    result.IsSuccess = false;
                    result.Message = "Chi tiết này đã tồn tại.";
                }
                else
                {
                    var detail = new ProductDetail
                    {
                        ProductId = dto.ProductId,
                        SizeId = item.SizeId,
                        ColorId = item.ColorId,
                        FabricId = item.FabricId
                    };

                    _context.ProductDetails.Add(detail);
                    await _context.SaveChangesAsync();

                    result.IsSuccess = true;
                    result.Message = "Thêm thành công.";
                    result.Detail = ProductMapper.ToCreateProductDetailResponse(detail);
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"Lỗi khi xử lý chi tiết: {ex.Message}";
            }

            response.Results.Add(result);
        }

        return response;
    }



    //up1
    public async Task<List<CreateImageResponse>> AddImagesToDetailAsync(int detailId, List<string> imageUrls)
	{
		var responses = new List<CreateImageResponse>();

		try
		{
			foreach (var url in imageUrls)
			{
				var path = Path.GetFileName(new Uri(url).LocalPath);
				bool isDuplicate = await _context.ImagesProducts
					.AnyAsync(i => i.ProductDetailId == detailId && i.Path == path);

				if (isDuplicate)
				{
					responses.Add(new CreateImageResponse
					{
						IsSuccess = false,
						Message = $"Ảnh với tên `{path}` đã tồn tại."
					});
					continue;
				}

				var image = new ImagesProduct
				{
					Path = path,
					FullPath = url,
					ProductDetailId = detailId
				};

				_context.ImagesProducts.Add(image);
				await _context.SaveChangesAsync();

				var res = ProductMapper.ToCreateImageResponse(image);
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




	///
	public async Task DeleteProductAsync(int productId)
	{
		var product = await _context.Products
			.Include(p => p.ProductDetails)
				.ThenInclude(d => d.Images)
			.FirstOrDefaultAsync(p => p.ProductId == productId);

		if (product == null)
			throw new Exception($"Sản phẩm với ID={productId} không tồn tại.");

		var detailIds = product.ProductDetails.Select(d => d.DetailId).ToList();
		// Kiểm tra trước khi xóa toàn bộ
		await ValidateDetailsNotInUseAsync(detailIds);

		foreach (var detail in product.ProductDetails)
			_context.ImagesProducts.RemoveRange(detail.Images);

		_context.ProductDetails.RemoveRange(product.ProductDetails);
		_context.Products.Remove(product);
		await _context.SaveChangesAsync();
	}
	public async Task DeleteProductDetailAsync(int detailId)
	{
		var detail = await GetDetailOrThrowAsync(detailId);
		await ValidateDetailsNotInUseAsync(new[] { detailId });

		_context.ImagesProducts.RemoveRange(detail.Images);
		_context.ProductDetails.Remove(detail);
		await _context.SaveChangesAsync();
	}

	//up1
	public async Task DeleteImageAsync(int imageId)
	{
		var image = await _context.ImagesProducts.FindAsync(imageId);
		if (image == null)
		{
			throw new Exception($"Ảnh với ID {imageId} không tồn tại.");
		}
		_context.ImagesProducts.Remove(image);
		await _context.SaveChangesAsync();
	}

	//validation
	private async Task ValidateCategoryExistsAsync(int categoryId)
	{
		var exists = await _context.CategoryProducts.AnyAsync(c => c.CategoryId == categoryId);
		if (!exists)
		{
			throw new Exception("Danh mục sản phẩm không hợp lệ!");
		}
	}
	private async Task ValidateProductDetailReferencesAsync(int sizeId, int colorId, int fabricId)
	{
		var sizeExists = await _context.Sizes.AnyAsync(s => s.SizeId == sizeId);
		var colorExists = await _context.Colors.AnyAsync(c => c.ColorId == colorId);
		var fabricExists = await _context.Fabrics.AnyAsync(f => f.FabricId == fabricId);

		if (!sizeExists) throw new Exception("Kích thước không tồn tại!");
		if (!colorExists) throw new Exception("Màu sắc không tồn tại!");
		if (!fabricExists) throw new Exception("Chất liệu không tồn tại!");

	}
	private async Task ValidateImageExistsAsync(int imageId, int productDetailId)
	{
		var exists = await _context.ImagesProducts
			.AnyAsync(i => i.Id == imageId && i.ProductDetailId == productDetailId);
		if (!exists)
		{
			throw new Exception($"Ảnh với ImageId={imageId} không tồn tại hoặc không thuộc về ProductDetailId={productDetailId}.");
		}
	}

	private async Task<ProductDetail> GetDetailOrThrowAsync(int detailId)
	{
		var detail = await _context.ProductDetails
			.Include(d => d.Images)
			.FirstOrDefaultAsync(d => d.DetailId == detailId);

		if (detail == null)
			throw new Exception($"Chi tiết sản phẩm với ID={detailId} không tồn tại.");


		return detail;
	}
	private async Task ValidateDetailsNotInUseAsync(IEnumerable<int> detailIds)
	{
		var idList = detailIds.ToList(); 

		foreach (var id in idList)
		{
			bool inInput = await _context.InputReceiptDetails.AnyAsync(x => x.ProductDetailId == id);
			bool inOutput = await _context.OutputReceiptDetails.AnyAsync(x => x.ProductDetailId == id);

			if (inInput || inOutput)
			{
				var parts = new List<string>();
				if (inInput) parts.Add("trong phiếu nhập");
				if (inOutput) parts.Add("trong phiếu xuất");
				throw new Exception($"Không thể thực hiện vì chi tiết sản phẩm với ID = {id} đang được sử dụng {string.Join(" và ", parts)}.");
			}
		}
	}

}

