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
        var category = new Category
        {
            Name = categoryRequest.Name,
            CreatedBy = "testeCreate",
            CreatedOn= DateTime.Now,
            EditedBy = "testeEdit",
            EditedOn= DateTime.Now,

        };
        context.Categories.Add(category);
        context.SaveChanges();

        return Results.Created($"{Template}/{category.Id}", category.Id);
    }

}
