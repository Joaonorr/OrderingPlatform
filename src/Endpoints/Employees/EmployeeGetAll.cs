using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using OrderingPlatform.Infra.Data;

namespace OrderingPlatform.Endpoints.Employees;


public static class EmployeeGetAll
{
    public static string Template => "/employee";
    public static string[] Method => new string[] { HttpMethod.Get.ToString() };
    public static Delegate Handle => Action;

    [Authorize]
    public static IResult Action(int? page, int? rows, QueryGetAllEmployeeWithClaimName query)
    {
       return Results.Ok(query.ExecuteQuery(page.Value, rows.Value));
    }
}
