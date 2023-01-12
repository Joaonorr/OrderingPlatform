using OrderingPlatform.Domain.Products;
using OrderingPlatform.Infra.Data;

namespace OrderingPlatform.Endpoints.Categories;

public class CategoryPost
{
    public static string Template => "/categories";
    public static string[] Method => new String[] { HttpMethod.Post.ToString() };
    public static Delegate Handle => Action;

    public static IResult Action(categoryRequest categoryRequest, ApplicationDbContext context)
    {
        var category = new Category(categoryRequest.Name, "teste1", "teste2");
     
        if (!category.IsValid)
        {
            return Results.ValidationProblem(category.Notifications.ConvertToProblemDetails());
        }
            

        context.Categories.Add(category);
        context.SaveChanges();

        return Results.Created($"{Template}/{category.Id}", category.Id);
    }

}
