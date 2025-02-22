using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.Products
{
    public class UpdateProductModel
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }

        public List<string> Categories { get; set; } = new List<string>();
    }
}
