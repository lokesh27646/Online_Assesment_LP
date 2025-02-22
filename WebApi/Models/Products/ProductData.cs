using System;
using System.Collections.Generic;
using BusinessEntities;

namespace WebApi.Models.Products
{
    public class ProductData
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public List<string> Categories { get; set; }

        public ProductData(Product product)
        {
            Id = product.Id;
            Name = product.Name;
            Description = product.Description;
            Price = product.Price;
            StockQuantity = product.StockQuantity;
            Categories = new List<string>(product.Categories);
        }
    }
}
