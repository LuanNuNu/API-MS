using MenShop_Assignment.Datas;
using MenShop_Assignment.DTOs;
using MenShop_Assignment.Models;

namespace MenShop_Assignment.Mapper
{
    public static class CategoryProductMapper
    {
        public static CategoryProductViewModel ToCategoryProductView(CategoryProduct categoryProduct)
        {
            return new CategoryProductViewModel
            {
                CategoryId = categoryProduct.CategoryId,
                Name = categoryProduct.Name ?? null
            };
        }
        public static CategoryProduct ToCategoryProduct(CreateUpdateCategoryDTO category)
        {
            return new CategoryProduct
            {
                Name = category.Name,
            };
        }
    }
}
