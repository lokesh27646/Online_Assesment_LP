using System;
using System.Collections.Generic;
using System.Web.Http;
using Microsoft.Owin;
using Owin;
using SimpleInjector;
using SimpleInjector.Integration.WebApi;
using BusinessEntities;
using Data;
using System.Web.Http.Cors;
using Newtonsoft.Json.Serialization;

[assembly: OwinStartup(typeof(WebApi.Startup))]

namespace WebApi
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var container = new Container();

            // Create singleton instances of repositories
            var userRepository = new UserRepository();
            var productRepository = new ProductRepository();
            var orderRepository = new OrderRepository();

            // Register repositories
            container.Register<IRepository<User>>(() => userRepository, Lifestyle.Singleton);
            container.Register<IRepository<Product>>(() => productRepository, Lifestyle.Singleton);
            container.Register<IRepository<Order>>(() => orderRepository, Lifestyle.Singleton);

            // Add some sample data
            AddSampleData(userRepository, productRepository, orderRepository);

            // Verify the container
            container.Verify();

            var config = new HttpConfiguration();
            
            // Enable CORS
            var cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);

            // Use camelCase for JSON
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            
            // Configure Web API routes
            config.MapHttpAttributeRoutes();
            
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Set the dependency resolver
            config.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(container);

            app.UseWebApi(config);
        }

        private void AddSampleData(IRepository<User> userRepo, IRepository<Product> productRepo, IRepository<Order> orderRepo)
        {
            // Add sample users
            var user1 = new User(Guid.NewGuid(), "John Doe", "john@example.com", 5000);
            var user2 = new User(Guid.NewGuid(), "Jane Smith", "jane@example.com", 6000);
            userRepo.Add(user1);
            userRepo.Add(user2);

            // Add sample products
            var product1 = new Product(
                Guid.NewGuid(),
                "Laptop",
                "High-performance laptop",
                999.99m,
                10,
                new List<string> { "Electronics", "Computers" }
            );
            var product2 = new Product(
                Guid.NewGuid(),
                "Smartphone",
                "Latest smartphone model",
                599.99m,
                20,
                new List<string> { "Electronics", "Phones" }
            );
            productRepo.Add(product1);
            productRepo.Add(product2);

            // Add sample orders
            var order1 = new Order(
                Guid.NewGuid(),
                user1.Id,
                new List<OrderItem>
                {
                    new OrderItem { ProductId = product1.Id, Quantity = 1, UnitPrice = product1.Price }
                }
            );
            var order2 = new Order(
                Guid.NewGuid(),
                user2.Id,
                new List<OrderItem>
                {
                    new OrderItem { ProductId = product2.Id, Quantity = 2, UnitPrice = product2.Price }
                }
            );
            orderRepo.Add(order1);
            orderRepo.Add(order2);
        }
    }
}
