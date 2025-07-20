using MenShop_Assignment.Datas;
using MenShop_Assignment.DTOs;
using MenShop_Assignment.Models;

namespace MenShop_Assignment.Repositories.Category
{
    public interface ICategoryProductRepository
    {
        Task<List<CategoryProductViewModel>?> GetAllCategoriesAsync();
        Task<CategoryProductViewModel?> GetCategoryByIdAsync(int id);
        Task<bool> RemoveCategoryAsync(int Id);
		Task<bool> CreateCategoryAsync(CreateUpdateCategoryDTO category);
		Task<bool> UpdateCategoryAsync(CreateUpdateCategoryDTO categoryDTO);
	}
}
