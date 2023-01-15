using Microsoft.AspNetCore.Authorization;
using OrderingPlatform.Domain.Products;
using OrderingPlatform.Infra.Data;
using System.Security.Claims;

namespace OrderingPlatform.Endpoints.Categories;

public class CategoryPost
{
    public static string Template => "/categories";
    public static string[] Method => new String[] { HttpMethod.Post.ToString() };
    public static Delegate Handle => Action;

    [Authorize(Policy = "EmployeePolicy")]
    public static IResult Action(categoryRequest categoryRequest, ApplicationDbContext context, HttpContext http)
    {
        var userId = http.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        var category = new Category(categoryRequest.Name, userId, userId);
     
        if (!category.IsValid)
        {
            return Results.ValidationProblem(category.Notifications.ConvertToProblemDetails());
        }
            
        context.Categories.Add(category);
        context.SaveChanges();

        return Results.Created($"{Template}/{category.Id}", category.Id);
    }

}
