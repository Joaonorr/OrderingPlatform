namespace OrderingPlatform.Endpoints.Employees;

public record EmployeeRequest(string Name, string Email, string Password, string employCode)
{
}
