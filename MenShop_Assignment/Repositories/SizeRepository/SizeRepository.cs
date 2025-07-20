using MenShop_Assignment.Datas;
using MenShop_Assignment.Mapper;
using MenShop_Assignment.Models;
using Microsoft.EntityFrameworkCore;

namespace MenShop_Assignment.Repositories.SizeRepositories
{
	public class SizeRepository : ISizeRepository
	{
		private readonly ApplicationDbContext _context;


		public SizeRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<List<SizeViewModel>> GetAllSize()
		{
			var sizeList = await _context.Sizes.ToListAsync();
			return sizeList.Select(SizeMapper.ToSizeViewModel).ToList();
		}

		public async Task<SizeViewModel> GetByIdSize(int Id)
		{
			var size = await _context.Sizes.Where(x => x.SizeId == Id).FirstOrDefaultAsync();
			return SizeMapper.ToSizeViewModel(size);
		}

        //up1
        public async Task<bool> CreateSize(string sizeName)
        {
            if (string.IsNullOrWhiteSpace(sizeName))
                return false;

            bool exists = await _context.Sizes.AnyAsync(s => s.Name == sizeName);
            if (exists)
                return false;

            _context.Sizes.Add(new Size { Name = sizeName });
            await _context.SaveChangesAsync();
            return true;
        }

        //up1
        public async Task<bool> UpdateSize(int id, string sizeName)
        {
            if (string.IsNullOrWhiteSpace(sizeName))
                return false;

            var size = await _context.Sizes.FindAsync(id);
            if (size == null)
                return false;
            bool exists = await _context.Sizes.AnyAsync(s => s.Name == sizeName && s.SizeId != id);
            if (exists)
                return false;

            size.Name = sizeName;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteSize(int Id)
		{
			var size = await _context.Sizes.FindAsync(Id);
			if (size == null)
				return false;

			_context.Sizes.Remove(size);
			await _context.SaveChangesAsync();
			return true;
		}
	}
}
