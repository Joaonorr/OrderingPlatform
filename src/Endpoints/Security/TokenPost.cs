using Microsoft.AspNetCore.Authorization;
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

    [AllowAnonymous]
    public static IResult Action(
        LoginRequest loginRequest, 
        UserManager<IdentityUser> userManager, 
        IConfiguration configuration
    )
    {
        var user = userManager.FindByEmailAsync(loginRequest.Email).Result;

        if (user == null)
            return Results.BadRequest();

        if (! userManager.CheckPasswordAsync(user, loginRequest.Password).Result)
            return Results.BadRequest();

        var key = Encoding.ASCII.GetBytes(configuration["JwtBearerTokenSettings:SecretKey"]);
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

            Audience = configuration["JwtBearerTokenSettings:Audience"],

            Issuer = configuration["JwtBearerTokenSettings:Issuer"],
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescription);


        return Results.Ok(new {token = tokenHandler.WriteToken(token)});

    }
}
