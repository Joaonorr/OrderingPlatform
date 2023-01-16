namespace OrderingPlatform.Endpoints.Products;

public record ProductResponse(Guid Id, string Name, string CategoryName, string Description, bool HasStock, bool Active);