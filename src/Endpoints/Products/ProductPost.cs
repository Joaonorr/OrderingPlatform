using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using OrderingPlatform.Domain.Products;
using OrderingPlatform.Infra.Data;
using System.Security.Claims;

namespace OrderingPlatform.Endpoints.Products;

public static class ProductPost
{
    public static string Template => "/product";
    public static string[] Method => new string[] {HttpMethod.Post.ToString() };
    public static Delegate Handle => Action;

    [Authorize(Policy = "EmployeePolicy")]
    public static async Task<IResult> Action(
        ProductRequest productRequest, HttpContext httpContext, ApplicationDbContext applicationDbContext)
    {
        var userId = httpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        var category = await applicationDbContext
            .Categories.FirstOrDefaultAsync(c => c.Id == productRequest.CategoryId);

        if (category == null)
            return Results.NotFound("category not found");

        var product = new Product(
             productRequest.Name, category, productRequest.Description, productRequest.Price, productRequest.HasStoock, userId
        );

        if (!product.IsValid)
            return Results.ValidationProblem(product.Notifications.ConvertToProblemDetails());

        await applicationDbContext.Products.AddAsync(product);
        await applicationDbContext.SaveChangesAsync();

        return Results.Created($"/products/{product.Id}", product.Id);
    }
}
