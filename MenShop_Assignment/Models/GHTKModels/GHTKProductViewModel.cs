using MenShop_Assignment.Datas;
using System.ComponentModel.DataAnnotations.Schema;

namespace MenShop_Assignment.Models.GHTKModel
{
    public class GHTKProductViewModel
    {
        public string ProductId { get; set; }
        public string? Name { get; set; }
        public double? Weight { get; set; } //Kg
    }
}
