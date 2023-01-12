﻿using Microsoft.AspNetCore.Identity;
using OrderingPlatform.Infra.Data;
using System.Security.Claims;

namespace OrderingPlatform.Endpoints.Employees;

public static class EmployeePost
{
    public static string Template => "/employee";
    public static string[] Method => new string[] { HttpMethod.Post.ToString() };
    public static Delegate Handle => Action;

    public static IResult Action(EmployeeRequest employeeRequest, UserManager<IdentityUser> userManager)
    {
        var user = new IdentityUser
        {
            UserName = employeeRequest.Email,
            Email = employeeRequest.Email,
        };

        var result = userManager.CreateAsync(user, employeeRequest.Password).Result;

        if (!result.Succeeded)
            return Results.ValidationProblem(result.Errors.ConvertToProblemDetails());

        var claims = new List<Claim>()
        {
            new Claim("EmploreeCode", employeeRequest.employCode),
            new Claim("Name", employeeRequest.Name)
        };

        var claimResult = userManager.AddClaimsAsync(user, claims).Result;

        if (!claimResult.Succeeded)
            return Results.BadRequest(result.Errors.ConvertToProblemDetails());


        return Results.Created($"/employee/{user.Id}", user.Id);
    }
}