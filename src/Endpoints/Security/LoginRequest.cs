namespace OrderingPlatform.Endpoints.Security;

public record LoginRequest(string Email, string Password)
{
}