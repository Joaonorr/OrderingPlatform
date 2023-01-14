using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OrderingPlatform.Endpoints.Security;

public static class TokenPost
{
    public static string Template => "/token";
    public static string[] Method => new string[]{ HttpMethod.Post.ToString() };
    public static Delegate Handle => Action;

    public static IResult Action(LoginRequest loginRequest, UserManager<IdentityUser> userManager)
    {
        var user = userManager.FindByEmailAsync(loginRequest.Email).Result;

        if (user == null)
            return Results.BadRequest();

        if (! userManager.CheckPasswordAsync(user, loginRequest.Password).Result)
            return Results.BadRequest();

        var key = Encoding.ASCII.GetBytes("Po]kxvmZ7oKA>ls*wB#AMAO<j~aXbF");
        var tokenDescription = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity
            (
                new Claim[]
                {
                    new Claim(ClaimTypes.Email, loginRequest.Email)
                }
            ),

            SigningCredentials = new SigningCredentials
            (
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            ),

            Audience = "OrderingPlataform",

            Issuer = "Issuer"
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescription);


        return Results.Ok(new {token = tokenHandler.WriteToken(token)});

    }
}
