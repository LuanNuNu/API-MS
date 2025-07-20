using MenShop_Assignment.Models;

namespace MenShop_Assignment.Repositories.ColorRepository
{
	public interface IColorRepository
	{
		Task<bool> CreateColor(string color);
		Task<bool> DeleteColor(int Id);
		Task<List<ColorViewModel>> GetAllColor();
		Task<ColorViewModel> GetByIdColor(int Id);
		Task<bool> UpdateColor(int Id, string newColor);
	}
}