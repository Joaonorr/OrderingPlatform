using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using OrderingPlatform.Domain.Users;
using System.Security.Claims;

namespace OrderingPlatform.Endpoints.Clients;

public class ClientGet
{
    public static string Template => "/clients";
    public static string[] Method => new string[] { HttpMethod.Get.ToString() };
    public static Delegate Handle => Action;

    //[Authorize(Policy = "EmployeePolicy")]
    [AllowAnonymous]
    public static async Task<IResult> Action(HttpContext context)
    {
        var user = context.User;
        var result = new
        {
            Id = user.Claims.First(user => user.Type == ClaimTypes.NameIdentifier).Value,
            Name = user.Claims.First(user => user.Type == "Name").Value,
            Cpf = user.Claims.First(user => user.Type == "Cpf").Value
        };

        return Results.Ok(result);
    }
}
