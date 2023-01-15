using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        EmployeeRequest employeeRequest, HttpContext http, UserManager<IdentityUser> userManager)
    {
        var userId = http.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        var user = new IdentityUser{ UserName = employeeRequest.Email, Email = employeeRequest.Email };
        var result = await userManager.CreateAsync(user, employeeRequest.Password);

        if (!result.Succeeded)
            return Results.ValidationProblem(result.Errors.ConvertToProblemDetails());

        var claims = new List<Claim>()
        {
            new Claim("EmployeeCode", employeeRequest.employCode),
            new Claim("Name", employeeRequest.Name),
            new Claim("CreateBy", userId)
        };

        var claimResult = await userManager.AddClaimsAsync(user, claims);

        if (!claimResult.Succeeded)
            return Results.BadRequest(result.Errors.ConvertToProblemDetails());


        return Results.Created($"/employee/{user.Id}", user.Id);
    }
}
