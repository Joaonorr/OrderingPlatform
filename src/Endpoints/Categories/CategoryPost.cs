using OrderingPlatform.Infra.Data;

namespace OrderingPlatform.Endpoints.Categories;

public class CategoryPost
{
    public static string Template => "/categories";
    public static string[] Method => new String[] { HttpMethod.Post.ToString() };
    public static Delegate Handle => Action;

    public static IResult Action(CategoryResponse categoryResponse, ApplicationDbContext context)
    {
        return Results.Ok("ok");
    }

}
