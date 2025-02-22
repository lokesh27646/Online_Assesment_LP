# Sample Project Web API

This is a .NET Framework 4.7.2 Web API project that demonstrates a simple e-commerce system with Users, Products, and Orders.

## Getting Started

1. Clone the repository
2. Build the solution: `dotnet build`
3. Run the WebApi project: `.\WebApi\bin\Debug\net472\WebApi.exe`
4. The API will be available at `http://localhost:9000`

## API Endpoints

### Users

- **Create User**
  - POST `/api/users`
  - Body: `{ "name": "John Doe", "email": "john@example.com", "monthlySalary": 5000 }`

- **Update User**
  - PUT `/api/users/{userId}`
  - Body: `{ "name": "John Updated", "email": "john.updated@example.com", "monthlySalary": 6000 }`

- **Delete User**
  - DELETE `/api/users/{userId}`

- **Get User by ID**
  - GET `/api/users/{userId}`

- **List Users**
  - GET `/api/users`
  - Query Parameters:
    - nameFilter (optional)
    - emailFilter (optional)
    - minSalary (optional)
    - maxSalary (optional)

### Products

- **Create Product**
  - POST `/api/products`
  - Body: `{ "name": "Sample Product", "description": "A sample product", "price": 29.99, "stockQuantity": 100, "categories": ["Electronics"] }`

- **Update Product**
  - PUT `/api/products/{productId}`
  - Body: `{ "name": "Updated Product", "description": "Updated description", "price": 39.99, "stockQuantity": 150, "categories": ["Electronics"] }`

- **Delete Product**
  - DELETE `/api/products/{productId}`

- **Get Product by ID**
  - GET `/api/products/{productId}`

- **List Products**
  - GET `/api/products`
  - Query Parameters:
    - nameFilter (optional)
    - minPrice (optional)
    - maxPrice (optional)

### Orders

- **Create Order**
  - POST `/api/orders`
  - Body: `{ "userId": "user-guid", "items": [{ "productId": "product-guid", "quantity": 2 }] }`

- **Update Order Status**
  - PUT `/api/orders/{orderId}/status`
  - Body: `{ "status": "Processing" }`

- **Delete Order**
  - DELETE `/api/orders/{orderId}`

- **Get Order by ID**
  - GET `/api/orders/{orderId}`

- **List Orders**
  - GET `/api/orders`
  - Query Parameters:
    - userId (optional)
    - status (optional)
    - fromDate (optional)
    - toDate (optional)

## Project Structure

- **WebApi**: API controllers and models
- **Core**: Business logic and services
- **Data**: Repository implementations
- **BusinessEntities**: Domain models
- **Common**: Shared utilities

## Storage

The project uses in-memory storage for demonstration purposes. Data will be lost when the application restarts.

## Testing

Use the provided Postman collection in the `postmanCollection` folder to test the API endpoints. Import the collection into Postman and set the `baseUrl` variable to `http://localhost:9000`.
