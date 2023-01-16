namespace OrderingPlatform.Endpoints.Products;

public record ProductResponse(Guid Id, string Name, decimal Price, string CategoryName, string Description, bool HasStock, bool Active);