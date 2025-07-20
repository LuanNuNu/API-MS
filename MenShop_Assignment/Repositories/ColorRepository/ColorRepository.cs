using System.Drawing;
using MenShop_Assignment.Datas;
using MenShop_Assignment.Mapper;
using MenShop_Assignment.Models;
using Microsoft.EntityFrameworkCore;

namespace MenShop_Assignment.Repositories.ColorRepository
{
	public class ColorRepository : IColorRepository
	{

		private readonly ApplicationDbContext _context;
		public ColorRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<List<ColorViewModel>> GetAllColor()
		{
			var colorList = await _context.Colors.ToListAsync();
			return colorList.Select(ColorMapper.ToColorViewModel).ToList() ?? [];
		}

		public async Task<ColorViewModel> GetByIdColor(int Id)
		{
			var color = await _context.Colors.Where(x => x.ColorId == Id).FirstOrDefaultAsync();
			return ColorMapper.ToColorViewModel(color);
		}

		public async Task<bool> CreateColor(string color)
		{
			if (color == null)
				return false;
			var colorList = await _context.Colors.ToListAsync();
			if (colorList.Any(x => x.Name == color))
				return false;

			_context.Colors.Add(new Datas.Color { Name = color });
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<bool> UpdateColor(int Id, string newColor)
		{
			var color = await _context.Colors.FindAsync(Id);
			if (color == null)
				return false;
            bool exists = await _context.Colors.AnyAsync(f => f.Name == newColor && f.ColorId != Id);
            if (exists)
                return false;
            color.Name = newColor;
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<bool> DeleteColor(int Id)
		{
			var color = await _context.Colors.FindAsync(Id);
			if (color == null)
				return false;

			_context.Colors.Remove(color);
			await _context.SaveChangesAsync();
			return true;
		}
	}
}
