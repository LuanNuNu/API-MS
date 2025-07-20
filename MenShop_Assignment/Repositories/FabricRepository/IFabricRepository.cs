using MenShop_Assignment.Models;

namespace MenShop_Assignment.Repositories.FabricRepositories
{
	public interface IFabricRepository
	{
		Task<bool> CreateFabric(string fabricName);
		Task<bool> DeleteFabric(int Id);
		Task<List<FabricViewModel>?> GetAllFabric();
		Task<FabricViewModel?> GetByIdFabric(int Id);
		Task<bool> UpdateFabric(int Id, string newFabric);
	}
}