using MenShop_Assignment.Datas;
using MenShop_Assignment.DTOs;
using MenShop_Assignment.Mapper;
using MenShop_Assignment.Models;
using Microsoft.EntityFrameworkCore;

namespace MenShop_Assignment.Repositories.Category
{
    public class CategoryProductRepository : ICategoryProductRepository
    {
        private readonly ApplicationDbContext _context;
        public CategoryProductRepository(ApplicationDbContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<List<CategoryProductViewModel>?> GetAllCategoriesAsync()
        {
            var categoryList = await _context.CategoryProducts.ToListAsync();
            if (categoryList == null) 
                return null;
            return categoryList.Select(CategoryProductMapper.ToCategoryProductView).ToList();
        }

        public async Task<CategoryProductViewModel?> GetCategoryByIdAsync(int id)
        {
            var categoryProduct = await _context.CategoryProducts.FirstOrDefaultAsync(x => x.CategoryId == id);
            if (categoryProduct == null) 
                return null;

            return CategoryProductMapper.ToCategoryProductView(categoryProduct);
        }

        //up1
        public async Task<bool> CreateCategoryAsync(CreateUpdateCategoryDTO categoryDTO)
        {
            if (categoryDTO == null || string.IsNullOrWhiteSpace(categoryDTO.Name))
                return false;

            var existed = await _context.CategoryProducts
                .AnyAsync(c => c.Name.ToLower() == categoryDTO.Name.Trim().ToLower());

            if (existed)
                return false; 

            var entity = CategoryProductMapper.ToCategoryProduct(categoryDTO);
            _context.CategoryProducts.Add(entity);
            await _context.SaveChangesAsync();


            _context.Storages.Add(new Storage { CategoryId = entity.CategoryId });
            await _context.SaveChangesAsync();

            return true;
        }


        public async Task<bool> UpdateCategoryAsync(CreateUpdateCategoryDTO categoryDTO)
        {
            var existing = await _context.CategoryProducts.FirstOrDefaultAsync(x => x.CategoryId == categoryDTO.Id);
            if (existing == null) 
                return false;

            existing.Name = categoryDTO.Name;
            _context.CategoryProducts.Update(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        //up1
        public async Task<bool> RemoveCategoryAsync(int id)
        {
            var category = await _context.CategoryProducts.FirstOrDefaultAsync(x => x.CategoryId == id);
            if (category == null) return false;

            var hasProducts = await _context.Products.AnyAsync(p => p.CategoryId == id);
            if (hasProducts) return false;

            var storage = await _context.Storages.FirstOrDefaultAsync(s => s.CategoryId == id);
            if (storage != null)
            {
                var hasStorageDetails = await _context.StorageDetails.AnyAsync(sd => sd.StorageId == storage.StorageId);
                if (hasStorageDetails) return false;

                _context.Storages.Remove(storage);
            }

            _context.CategoryProducts.Remove(category);
            await _context.SaveChangesAsync();

            return true;
        }


    }
}
