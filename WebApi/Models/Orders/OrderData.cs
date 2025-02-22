using System;
using System.Collections.Generic;
using System.Linq;
using BusinessEntities;

namespace WebApi.Models.Orders
{
    public class OrderData
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Status { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderItemData> Items { get; set; }

        public OrderData(Order order)
        {
            Id = order.Id;
            UserId = order.UserId;
            Status = order.Status;
            OrderDate = order.OrderDate;
            TotalAmount = order.TotalAmount;
            Items = order.Items.Select(i => new OrderItemData(i)).ToList();
        }
    }

    public class OrderItemData
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice => Quantity * UnitPrice;

        public OrderItemData(OrderItem item)
        {
            ProductId = item.ProductId;
            Quantity = item.Quantity;
            UnitPrice = item.UnitPrice;
        }
    }
}
