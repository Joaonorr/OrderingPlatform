using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderingPlatform.Infra.Data;

namespace OrderingPlatform.Endpoints.Categories
{
    public class CategoryPut
    {
        public static string Template => "category/{id:guid}";
        public static string[] Methods => new string[] { HttpMethod.Put.ToString() };
        public static Delegate Handle => Action;

        [Authorize(Policy = "EmployeePolicy")]
        public static IResult Action([FromRoute] Guid id, categoryRequest categoryRequest, ApplicationDbContext context, HttpContext http)
        {
            var userId = http.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var category = context.Categories.Where(c => c.Id == id).FirstOrDefault();

            if (category == null)
                return Results.NotFound();

            category.EditInfo(categoryRequest.Name, categoryRequest.Active, userId);

            if (!category.IsValid)
            {
                return Results.ValidationProblem(category.Notifications.ConvertToProblemDetails());
            }

            context.SaveChanges();

            return Results.Ok();
        }

    }
}
