using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessEntities
{
    public class OrderStatus
    {
        public const string Pending = "Pending";
        public const string Processing = "Processing";
        public const string Shipped = "Shipped";
        public const string Delivered = "Delivered";
        public const string Cancelled = "Cancelled";
    }

    public class OrderItem
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public class Order : IdObject
    {
        private Guid _userId;
        private List<OrderItem> _items = new List<OrderItem>();
        private string _status;
        private DateTime _orderDate;
        private decimal _totalAmount;

        public Guid UserId
        {
            get => _userId;
            private set => _userId = value;
        }

        public IEnumerable<OrderItem> Items
        {
            get => _items;
            private set => _items = new List<OrderItem>(value);
        }

        public string Status
        {
            get => _status;
            private set => _status = value;
        }

        public DateTime OrderDate
        {
            get => _orderDate;
            private set => _orderDate = value;
        }

        public decimal TotalAmount
        {
            get => _totalAmount;
            private set => _totalAmount = value;
        }

        public Order(Guid id, Guid userId, IEnumerable<OrderItem> items)
            : base(id)
        {
            UserId = userId;
            Items = items;
            Status = OrderStatus.Pending;
            OrderDate = DateTime.UtcNow;
            TotalAmount = items.Sum(i => i.UnitPrice * i.Quantity);
        }

        public void Update(IEnumerable<OrderItem> items)
        {
            Items = items;
            TotalAmount = items.Sum(i => i.UnitPrice * i.Quantity);
        }

        public void UpdateStatus(string status)
        {
            Status = status;
        }
    }
}
