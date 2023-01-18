using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Template;
using OrderingPlatform.Infra.Data;
using System.Security.Claims;

namespace OrderingPlatform.Endpoints.Orders;

public static class OrderById
{
    public static string Template => "order/{id}";
    public static string[] Method => new string[] { HttpMethod.Get.ToString() };
    public static Delegate Handle => Action;

    [Authorize(Policy = "SearchPolicy")]
    public static async Task<IResult> Action(
        [FromRoute] Guid id, 
        HttpContext httpContext, 
        ApplicationDbContext applicationDbContext)
    {
        if (id == Guid.Empty)
            return Results.BadRequest("OrderId field cannot be null");

        var order = applicationDbContext.Orders.FirstOrDefault(o => o.Id == id);

        if (order == null)
            return Results.NotFound("No product was found with that id");

        var userType = httpContext.User.Claims.First(c => c.Type == "UserType").Value;

        

        if ( userType == "Employee")
        {
            var orderResponse = 
                new OrderResponse(order.Id, new Guid(order.ClientId), order.Products, order.Total, order.DeliveryAddress);

            return Results.Ok(orderResponse);
        }
        

        Guid userId = new Guid(httpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);

        if ( id.Equals(order.Id) && userType == "Client")
        {
            var orderResponse = 
                new OrderResponse(order.Id, new Guid(order.ClientId), order.Products, order.Total, order.DeliveryAddress);

            return Results.Ok(orderResponse);
        }
        

        return Results.Problem("User does not have permission to access this route", statusCode: 401);
    }
}
