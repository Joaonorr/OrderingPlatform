using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Template;
using OrderingPlatform.Infra.Data;

namespace OrderingPlatform.Endpoints.Products;

public static class ProductGetById
{
    public static string Template => "/product/{id}";
    public static string[] Method => new string[] { HttpMethod.Get.ToString() };
    public static Delegate Handle => Action;

    [Authorize(Policy = "EmployeePolicy")]
    public static IResult Action([FromRoute] Guid id, ApplicationDbContext context)
    {
        var product = context.Products.First(p => p.Id == id);

        if (product == null)
            return Results.NotFound();

        return Results.Ok(product);
    }
}
