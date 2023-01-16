using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using OrderingPlatform.Domain.Users;
using OrderingPlatform.Endpoints.Employees;
using System.Security.Claims;

namespace OrderingPlatform.Endpoints.Clients;
public class ClientPost
{
    public static string Template => "/clients";
    public static string[] Method => new string[] { HttpMethod.Post.ToString() };
    public static Delegate Handle => Action;

    [AllowAnonymous]
    public static async Task<IResult> Action(
       ClientRequest clientRequest, UserCreator userCreator)
    {

        var userClaims = new List<Claim>()
        {
            new Claim("Cpf", clientRequest.Cpf),
            new Claim("Name", clientRequest.Name),
        };

        (IdentityResult identityResult, string UserId) result =
            await userCreator.Create(clientRequest.Email, clientRequest.Password, userClaims);

        if (!result.identityResult.Succeeded)
            return Results.ValidationProblem(result.identityResult.Errors.ConvertToProblemDetails());


        return Results.Created($"/employee/{result.UserId}", result.UserId);
    }
}
