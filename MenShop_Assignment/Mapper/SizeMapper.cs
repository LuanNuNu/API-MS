using MenShop_Assignment.Datas;
using MenShop_Assignment.Models;

namespace MenShop_Assignment.Mapper
{
    public static class SizeMapper
    {
        public static SizeViewModel ToSizeViewModel(Size size)
        {
            return new SizeViewModel
            {
                Name = size.Name ?? null,
                SizeId = size.SizeId,
            };
        }
    }
}
