using MenShop_Assignment.Datas;
using MenShop_Assignment.Models;
namespace MenShop_Assignment.Mapper
{
    public class FabricMapper
    {
        public static FabricViewModel ToFabricViewModel(Fabric fabric)
        {
            return new FabricViewModel
            {
                FabricId = fabric.FabricId,
                Name = fabric.Name ?? null,
            };
        }
    }
}
