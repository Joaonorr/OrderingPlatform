using Microsoft.AspNetCore.Mvc;
using OrderingPlatform.Infra.Data;

namespace OrderingPlatform.Endpoints.Categories
{
    public class CategoryPut
    {
        public static string Template => "categories/{id}";
        public static string[] Methods => new string[] { HttpMethod.Put.ToString() };
        public static Delegate Handle => Action;

        public static IResult Action([FromRoute] Guid id, categoryRequest categoryRequest, ApplicationDbContext context)
        {
            var category = context.Categories.Where(c => c.Id == id).FirstOrDefault();
            category.Name = categoryRequest.Name;
            category.Active = categoryRequest.Active;
            context.SaveChanges();

            return Results.Ok();
        }

    }
}
