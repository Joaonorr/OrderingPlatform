namespace OrderingPlatform.Endpoints.Products;

public record ProductRequest(string Name, Guid CategoryId, string Description, bool HasStoock, decimal Price, bool Active);