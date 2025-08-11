using MenShop_Assignment.Datas;
using MenShop_Assignment.Models;
using MenShop_Assignment.Models.ProductModels.ReponseDTO;
using static System.Net.Mime.MediaTypeNames;

namespace MenShop_Assignment.Mapper
{
    public class CollectionMapper
    {
        public static CollectionViewModel ToCollectionViewModel(Collection collection)
        {
            return new CollectionViewModel
            {
                CollectionId = collection.CollectionId,
                CollectionName = collection.CollectionName,
                Description = collection.Description,
                StartTime = collection.StartTime,
                EndTime = collection.EndTime,
                Status = collection.Status,
                Images = collection.Images
            };
        }

        public static CollectionDetailsViewModel ToProductCollectionViewModel(CollectionDetail c)
        {
            return new CollectionDetailsViewModel
            {
                CollectionDetailId = c.CollectionDetailId,
                CollectionId = c.CollectionId,
                ProductId = c.ProductId,
                ProductName = c.Product?.ProductName,
                ProductDetails = c.Product?.ProductDetails?
                    .Select(pd => new ProductDetailViewModel { DetailId = pd.DetailId })
                    .ToList() ?? new()
            };
        }


        public static ImageCollectionViewModel ToImageViewModel(ImageCollection img)
        {
            return new ImageCollectionViewModel
            {
                Id = img.Id,
                FullPath = string.IsNullOrEmpty(img.FullPath)
                    ? $"http://localhost:5014/StaticFiles/Images/{img.Path}"
                    : img.FullPath,
                CollectionId = img.CollectionId};
        }
        public static CreateImageResponse ToCreateImageResponse(ImageCollection image)
        {
            return new CreateImageResponse
            {
                ImageId = image.Id,
                ProductDetailId = image.CollectionId,
                ImageUrl = string.IsNullOrEmpty(image.FullPath)
                    ? $"http://localhost:5014/StaticFiles/Images/{image.Path}"
                    : image.FullPath
            };
        }
    }
}