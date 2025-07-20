using System.ComponentModel.DataAnnotations.Schema;

namespace MenShop_Assignment.Datas
{
    [NotMapped]
    public class GHTKProduct
    {
        public string? ProductId { get; set; }
        public string? Name { get; set; }
        public double? Weight { get; set; } //Kg
    }
}
