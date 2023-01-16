using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using OrderingPlatform.Domain.Users;
using OrderingPlatform.Endpoints.Clients;
using OrderingPlatform.Infra.Data;
using System.Security.Claims;

namespace OrderingPlatform.Endpoints.Employees;

public static class EmployeePost
{
    public static string Template => "/employee";
    public static string[] Method => new string[] { HttpMethod.Post.ToString() };
    public static Delegate Handle => Action;

    [Authorize(Policy = "EmployeePolicy")]
    public static async Task<IResult> Action(
        EmployeeRequest employeeRequest, HttpContext http, UserCreator userCreator)
    {
        var userId = http.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

        var claims = new List<Claim>()
        {
            new Claim("EmployeeCode", employeeRequest.employCode),
            new Claim("Name", employeeRequest.Name),
            new Claim("CreateBy", userId)
        };

        (IdentityResult identityResult, string UserId) result =
        await userCreator.Create(employeeRequest.Email, employeeRequest.Password, claims);

        if (!result.identityResult.Succeeded)
            return Results.ValidationProblem(result.identityResult.Errors.ConvertToProblemDetails());


        return Results.Created($"/employee/{result.UserId}", result.UserId);
    }
}
