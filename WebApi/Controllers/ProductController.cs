using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using BusinessEntities;
using Data;
using WebApi.Models.Products;
using System.Runtime.Serialization;

namespace WebApi.Controllers
{
    public class ProductController : ApiController
    {
        private readonly IRepository<Product> _productRepository;
        private readonly ProductRepository _productRepositoryFiltered;

        public ProductController(IRepository<Product> productRepository)
        {
            _productRepository = productRepository;
            _productRepositoryFiltered = productRepository as ProductRepository;
        }

        [HttpPost]
        [Route("api/products")]
        public IHttpActionResult Create([FromBody] CreateProductModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var product = new Product(
                    Guid.NewGuid(),
                    model.Name,
                    model.Description,
                    model.Price,
                    model.StockQuantity,
                    model.Categories
                );

                _productRepository.Add(product);
                return Ok(new ProductData(product));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut]
        [Route("api/products/{id}")]
        public IHttpActionResult Update(Guid id, [FromBody] UpdateProductModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var existingProduct = _productRepository.GetById(id);
                if (existingProduct == null)
                    return NotFound();

                var updatedProduct = new Product(
                    existingProduct.Id,
                    model.Name,
                    model.Description,
                    model.Price,
                    model.StockQuantity,
                    model.Categories
                );

                _productRepository.Update(updatedProduct);
                return Ok(new ProductData(updatedProduct));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpDelete]
        [Route("api/products/{id}")]
        public IHttpActionResult Delete(Guid id)
        {
            try
            {
                var product = _productRepository.GetById(id);
                if (product == null)
                    return NotFound();

                _productRepository.Delete(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("api/products/{id}")]
        public IHttpActionResult Get(Guid id)
        {
            try
            {
                var product = _productRepository.GetById(id);
                if (product == null)
                    return NotFound();

                return Ok(new ProductData(product));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("api/products")]
        public IHttpActionResult List(string nameFilter = null, decimal? minPrice = null, decimal? maxPrice = null, string category = null)
        {
            try
            {
                if (_productRepositoryFiltered != null)
                {
                    var products = _productRepositoryFiltered.GetFiltered(nameFilter, minPrice, maxPrice, category);
                    return Ok(products.Select(p => new ProductData(p)));
                }
                else
                {
                    var products = _productRepository.GetAll()
                        .Where(p => 
                            (string.IsNullOrEmpty(nameFilter) || p.Name.Contains(nameFilter)) &&
                            (!minPrice.HasValue || p.Price >= minPrice.Value) &&
                            (!maxPrice.HasValue || p.Price <= maxPrice.Value) &&
                            (string.IsNullOrEmpty(category) || p.Categories.Contains(category))
                        )
                        .ToList();

                    return Ok(products.Select(p => new ProductData(p)));
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }

    [DataContract]
    public class ProductData
    {
        [DataMember]
        public string Id { get; set; }
        
        [DataMember]
        public string Name { get; set; }
        
        [DataMember]
        public string Description { get; set; }
        
        [DataMember]
        public decimal Price { get; set; }
        
        [DataMember]
        public int StockQuantity { get; set; }
        
        [DataMember]
        public IEnumerable<string> Categories { get; set; }

        public ProductData()
        {
            Categories = new List<string>();
        }

        public ProductData(Product product)
        {
            Id = product.Id.ToString();
            Name = product.Name;
            Description = product.Description;
            Price = product.Price;
            StockQuantity = product.StockQuantity;
            Categories = product.Categories;
        }
    }
}
