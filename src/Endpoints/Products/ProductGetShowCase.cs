using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using OrderingPlatform.Infra.Data;
using System.Reflection;

namespace OrderingPlatform.Endpoints.Products;

public static class ProductGetShowCase
{
    public static string Template => "/product/showcase";
    public static string[] Method => new string[] { HttpMethod.Get.ToString() };
    public static Delegate Handle => Action;

    [AllowAnonymous]
    public static async Task<IResult> Action(ApplicationDbContext context, int page = 1, int row = 10, string orderBy = "name")
    {
        if (row < 1 || page < 1)
            return Results.Problem(
                title: "the number of pages and lines must be a positive integer greater than zero",
                statusCode: 400
            );

        if (row > 15)
            return Results.Problem(title: "Row with max 15", statusCode: 400);

        var query = context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Where(p => p.HasStoock && p.Category.Active);

        if (orderBy == "price")
            query = query.OrderByDescending(p => p.Price);
        else if (orderBy == "name")
            query = query.OrderBy(p => p.Name);
        else
            return Results.Problem(title: "Order only by price or name", statusCode: 400);

        query = query.Skip((page - 1) * row).Take(row); 

        var result = query.Select(
            p => new ProductResponse(p.Id ,p.Name, p.Price, p.Category.Name, p.Description, p.HasStoock, p.Active)
        );

        return Results.Ok(result);

    }
}
