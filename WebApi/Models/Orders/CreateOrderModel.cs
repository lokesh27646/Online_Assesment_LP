using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.Orders
{
    public class CreateOrderModel
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        [MinLength(1)]
        public List<OrderItemModel> Items { get; set; } = new List<OrderItemModel>();
    }

    public class OrderItemModel
    {
        [Required]
        public Guid ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }

    public class UpdateOrderStatusModel
    {
        [Required]
        public string Status { get; set; }
    }
}
