using MenShop_Assignment.Datas;
using MenShop_Assignment.Mapper;
using MenShop_Assignment.Models;
using Microsoft.EntityFrameworkCore;

namespace MenShop_Assignment.Repositories.FabricRepositories
{
	public class FabricRepository : IFabricRepository
	{
		private readonly ApplicationDbContext _context;

		public FabricRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<List<FabricViewModel>?> GetAllFabric()
		{
			var fabricList = await _context.Fabrics.ToListAsync();
			return fabricList.Select(FabricMapper.ToFabricViewModel).ToList() ?? [];
		}

		public async Task<FabricViewModel?> GetByIdFabric(int Id)
		{
			var fabric = await _context.Fabrics.Where(x => x.FabricId == Id).FirstOrDefaultAsync();
			if (fabric == null)
				return null;
			return FabricMapper.ToFabricViewModel(fabric);
		}

		//up1
        public async Task<bool> CreateFabric(string fabricName)
        {
            if (string.IsNullOrWhiteSpace(fabricName))
                return false;

            bool exists = await _context.Fabrics.AnyAsync(f => f.Name == fabricName);
            if (exists)
                return false;

            _context.Fabrics.Add(new Fabric { Name = fabricName });
            await _context.SaveChangesAsync();
            return true;
        }

		//up1
        public async Task<bool> UpdateFabric(int Id, string newFabric)
        {
            if (string.IsNullOrWhiteSpace(newFabric))
                return false;

            var fabric = await _context.Fabrics.FindAsync(Id);
            if (fabric == null)
                return false;
            bool exists = await _context.Fabrics.AnyAsync(f => f.Name == newFabric && f.FabricId != Id);
            if (exists)
                return false;

            fabric.Name = newFabric;
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> DeleteFabric(int Id)
		{
			var fabric = await _context.Fabrics.FindAsync(Id);
			if (fabric == null)
				return false;

			_context.Fabrics.Remove(fabric);
			await _context.SaveChangesAsync();
			return true;
		}
	}
}
