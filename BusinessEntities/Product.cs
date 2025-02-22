using System;
using System.Collections.Generic;

namespace BusinessEntities
{
    public class Product : IdObject
    {
        private string _name;
        private string _description;
        private decimal _price;
        private int _stockQuantity;
        private List<string> _categories = new List<string>();

        public string Name
        {
            get => _name;
            private set => _name = value;
        }

        public string Description
        {
            get => _description;
            private set => _description = value;
        }

        public decimal Price
        {
            get => _price;
            private set => _price = value;
        }

        public int StockQuantity
        {
            get => _stockQuantity;
            private set => _stockQuantity = value;
        }

        public IEnumerable<string> Categories
        {
            get => _categories;
            private set => _categories = new List<string>(value);
        }

        public Product(Guid id, string name, string description, decimal price, int stockQuantity, IEnumerable<string> categories = null)
            : base(id)
        {
            Name = name;
            Description = description;
            Price = price;
            StockQuantity = stockQuantity;
            Categories = categories ?? new List<string>();
        }

        public void Update(string name, string description, decimal price, int stockQuantity, IEnumerable<string> categories)
        {
            Name = name;
            Description = description;
            Price = price;
            StockQuantity = stockQuantity;
            Categories = categories ?? new List<string>();
        }
    }
}
