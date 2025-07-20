using MenShop_Assignment.Datas;
using MenShop_Assignment.Models;

namespace MenShop_Assignment.Mapper
{
	public static class ColorMapper
	{
		public static ColorViewModel ToColorViewModel(Color color)
		{
			return new ColorViewModel
			{
				ColorId = color.ColorId,
				Name = color.Name ?? null,
			};
		}
	}
}
