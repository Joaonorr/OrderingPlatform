using Microsoft.EntityFrameworkCore;
using OrderingPlatform.Infra.Data;

namespace OrderingPlatform.Endpoints.Products;

public static class ProductGetAll
{
    public static string Template => "/product";
    public static string[] Method => new string[] { HttpMethod.Get.ToString() };
    public static Delegate Handle => Action;

    public static async Task<IResult> Action(ApplicationDbContext context)
    {
        var products = context.Products.Include(p => p.Category).OrderBy(p => p.Name).ToList();
        var result = products.Select(
            p => new ProductResponse(p.Id ,p.Name, p.Category.Name, p.Description, p.HasStoock, p.Active)
        );

        return Results.Ok(result);

    }
}
