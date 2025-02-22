using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using BusinessEntities;
using Data;
using WebApi.Models.Orders;
using System.Runtime.Serialization;

namespace WebApi.Controllers
{
    public class OrderController : ApiController
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly OrderRepository _orderRepositoryFiltered;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<User> _userRepository;

        public OrderController(IRepository<Order> orderRepository, IRepository<Product> productRepository, IRepository<User> userRepository)
        {
            _orderRepository = orderRepository;
            _orderRepositoryFiltered = orderRepository as OrderRepository;
            _productRepository = productRepository;
            _userRepository = userRepository;
        }

        [HttpPost]
        [Route("api/orders")]
        public IHttpActionResult Create([FromBody] CreateOrderModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var user = _userRepository.GetById(model.UserId);
                if (user == null)
                    return BadRequest("Invalid user ID");

                var orderItems = new List<OrderItem>();
                foreach (var item in model.Items)
                {
                    var product = _productRepository.GetById(item.ProductId);
                    if (product == null)
                        return BadRequest($"Product not found: {item.ProductId}");

                    if (product.StockQuantity < item.Quantity)
                        return BadRequest($"Insufficient stock for product: {product.Name}");

                    orderItems.Add(new OrderItem 
                    { 
                        ProductId = item.ProductId, 
                        Quantity = item.Quantity,
                        UnitPrice = product.Price
                    });
                }

                var orderId = Guid.NewGuid();
                var order = new Order(orderId, model.UserId, orderItems);
                _orderRepository.Add(order);

                // Update product stock
                foreach (var item in orderItems)
                {
                    var product = _productRepository.GetById(item.ProductId);
                    var updatedProduct = new Product(
                        product.Id,
                        product.Name,
                        product.Description,
                        product.Price,
                        product.StockQuantity - item.Quantity,
                        product.Categories
                    );
                    _productRepository.Update(updatedProduct);
                }

                return Ok(new OrderData(order));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut]
        [Route("api/orders/{id}/status")]
        public IHttpActionResult UpdateStatus(Guid id, [FromBody] UpdateOrderStatusModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var order = _orderRepository.GetById(id);
                if (order == null)
                    return NotFound();

                order.UpdateStatus(model.Status);
                _orderRepository.Update(order);

                return Ok(new OrderData(order));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpDelete]
        [Route("api/orders/{id}")]
        public IHttpActionResult Delete(Guid id)
        {
            try
            {
                var order = _orderRepository.GetById(id);
                if (order == null)
                    return NotFound();

                // Restore product stock
                foreach (var item in order.Items)
                {
                    var product = _productRepository.GetById(item.ProductId);
                    var updatedProduct = new Product(
                        product.Id,
                        product.Name,
                        product.Description,
                        product.Price,
                        product.StockQuantity + item.Quantity,
                        product.Categories
                    );
                    _productRepository.Update(updatedProduct);
                }

                _orderRepository.Delete(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("api/orders/{id}")]
        public IHttpActionResult Get(Guid id)
        {
            try
            {
                var order = _orderRepository.GetById(id);
                if (order == null)
                    return NotFound();

                return Ok(new OrderData(order));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("api/orders")]
        public IHttpActionResult List(Guid? userId = null, string status = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                if (_orderRepositoryFiltered != null)
                {
                    var orders = _orderRepositoryFiltered.GetFiltered(userId, status, fromDate, toDate);
                    return Ok(orders.Select(o => new OrderData(o)));
                }
                else
                {
                    var orders = _orderRepository.GetAll()
                        .Where(o =>
                            (!userId.HasValue || o.UserId == userId.Value) &&
                            (string.IsNullOrEmpty(status) || o.Status == status) &&
                            (!fromDate.HasValue || o.OrderDate >= fromDate.Value) &&
                            (!toDate.HasValue || o.OrderDate <= toDate.Value)
                        )
                        .ToList();

                    return Ok(orders.Select(o => new OrderData(o)));
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }

    [DataContract]
    public class OrderData
    {
        [DataMember]
        public string Id { get; set; }
        
        [DataMember]
        public string UserId { get; set; }
        
        [DataMember]
        public string Status { get; set; }
        
        [DataMember]
        public DateTime OrderDate { get; set; }
        
        [DataMember]
        public decimal TotalAmount { get; set; }
        
        [DataMember]
        public IEnumerable<OrderItemData> Items { get; set; }

        public OrderData()
        {
            Items = new List<OrderItemData>();
        }

        public OrderData(Order order)
        {
            Id = order.Id.ToString();
            UserId = order.UserId.ToString();
            Status = order.Status;
            OrderDate = order.OrderDate;
            TotalAmount = order.Items.Sum(i => i.UnitPrice * i.Quantity);
            Items = order.Items.Select(i => new OrderItemData(i));
        }
    }

    [DataContract]
    public class OrderItemData
    {
        [DataMember]
        public string ProductId { get; set; }
        
        [DataMember]
        public int Quantity { get; set; }
        
        [DataMember]
        public decimal UnitPrice { get; set; }
        
        [DataMember]
        public decimal TotalPrice { get; set; }

        public OrderItemData()
        {
        }

        public OrderItemData(OrderItem item)
        {
            ProductId = item.ProductId.ToString();
            Quantity = item.Quantity;
            UnitPrice = item.UnitPrice;
            TotalPrice = item.UnitPrice * item.Quantity;
        }
    }
}
