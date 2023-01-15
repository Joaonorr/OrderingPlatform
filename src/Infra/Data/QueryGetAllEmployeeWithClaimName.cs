using Dapper;
using Microsoft.Data.SqlClient;
using OrderingPlatform.Endpoints.Employees;

namespace OrderingPlatform.Infra.Data;

public class QueryGetAllEmployeeWithClaimName
{
    private readonly IConfiguration configuration;

    public QueryGetAllEmployeeWithClaimName(IConfiguration configuration)
	{
		this.configuration = configuration;
	}

	public async Task<IEnumerable<EmployeeResponse>> ExecuteQuery(int page, int rows)
	{
        var db = new SqlConnection(configuration["ConnectionStrings:OrderingPlatform"]);

        var query = @"
            SELECT email, Claimvalue as Name
            from AspNetUsers INNER JOIN AspNetUserClaims
            ON 
                AspNetUsers.Id = AspNetUserClaims.UserId 
            AND 
                AspNetUserClaims.ClaimType = 'Name' 
            ORDER BY name
            OFFSET (@page - 1) * @rows ROWS FETCH NEXT @rows ROWS ONLY
        ";

        var employees = await db.QueryAsync<EmployeeResponse>(query, new { page, rows });

        return employees;
    }

}
