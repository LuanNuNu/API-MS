using MenShop_Assignment.Models;

namespace MenShop_Assignment.Repositories.SizeRepositories
{
	public interface ISizeRepository
	{
		Task<bool> CreateSize(string sizeName);
		Task<bool> DeleteSize(int Id);
		Task<List<SizeViewModel>> GetAllSize();
		Task<SizeViewModel> GetByIdSize(int Id);
		Task<bool> UpdateSize(int id, string sizeName);
	}
}