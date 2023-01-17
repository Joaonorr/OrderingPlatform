using Microsoft.AspNetCore.Authorization;
using OrderingPlatform.Domain.Orders;
using OrderingPlatform.Domain.Products;
using OrderingPlatform.Infra.Data;
using System.Security.Claims;

namespace OrderingPlatform.Endpoints.Orders;

public static class OrderPost
{
    public static string Template => "/orders";
    public static string[] Method => new string[] { HttpMethod.Post.ToString() };
    public static Delegate Handle => Action;

    [Authorize]
    public static async Task<IResult> Action(
        OrderRequest orderRequest, 
        HttpContext httpContext, 
        ApplicationDbContext applicationDbContext)
    {
        var clientId = httpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        var clientName = httpContext.User.Claims.First(c => c.Type == "Name").Value;

        List<Product> productsFound = null;

        if (orderRequest.ProductsIds != null && orderRequest.ProductsIds.Any())
            productsFound = applicationDbContext.Products.Where(p => orderRequest.ProductsIds.Contains(p.Id)).ToList();

        var order = new Order(clientId, clientName, productsFound, orderRequest.DeliveryAddress);

        if (!order.IsValid)        
            return Results.ValidationProblem(order.Notifications.ConvertToProblemDetails());

        return Results.Created($"/order/{order.Id}", order.Id);
    }
}
