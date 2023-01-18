using OrderingPlatform.Domain.Products;

namespace OrderingPlatform.Endpoints.Orders;

public record OrderResponse(Guid Id, Guid ClientId, List<Product> Products, decimal TotalPrice, string DeliveryAddress);