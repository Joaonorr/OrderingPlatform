using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using OrderingPlatform.Domain.Users;
using OrderingPlatform.Endpoints.Categories;
using OrderingPlatform.Endpoints.Clients;
using OrderingPlatform.Endpoints.Employees;
using OrderingPlatform.Endpoints.Orders;
using OrderingPlatform.Endpoints.Products;
using OrderingPlatform.Endpoints.Security;
using OrderingPlatform.Infra.Data;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .WriteTo.Console()
        .WriteTo.MSSqlServer(
            context.Configuration["ConnectionStrings:OrderingPlatform"],
            sinkOptions: new MSSqlServerSinkOptions()
            {
                  AutoCreateSqlTable = true,
                  TableName = "LogAPI"

            });
});

builder.Services.AddSqlServer<ApplicationDbContext>(builder
    .Configuration["ConnectionStrings:OrderingPlatform"]);
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 3;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireDigit = false;
}).AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthorization(options =>
{
    //options.FallbackPolicy = new AuthorizationPolicyBuilder()
    //  .AddAuthenticationSchemes
    //    (
    //        JwtBearerDefaults.AuthenticationScheme
    //    )
    //  .RequireAuthenticatedUser()
    //  .Build();
    options.AddPolicy("EmployeePolicy", p =>
            p.RequireAuthenticatedUser().RequireClaim("EmployeeCode"));
    options.AddPolicy("SearchPolicy", p =>
            p.RequireAuthenticatedUser().RequireClaim("UserType"));
});

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateActor = true,
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtBearerTokenSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtBearerTokenSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey
            (
                Encoding.UTF8.GetBytes(builder.Configuration["JwtBearerTokenSettings:SecretKey"])
            )
    };
});

builder.Services.AddScoped<QueryGetAllEmployeeWithClaimName>();
builder.Services.AddScoped<UserCreator>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapMethods(CategoryPost.Template, CategoryPost.Method, CategoryPost.Handle);
app.MapMethods(CategoryGetAll.Template, CategoryGetAll.Methods, CategoryGetAll.Handle);
app.MapMethods(CategoryPut.Template, CategoryPut.Methods, CategoryPut.Handle);

app.MapMethods(EmployeePost.Template, EmployeePost.Method, EmployeePost.Handle);
app.MapMethods(EmployeeGetAll.Template, EmployeeGetAll.Method, EmployeeGetAll.Handle);

app.MapMethods(ClientPost.Template, ClientPost.Method, ClientPost.Handle);
app.MapMethods(ClientGet.Template, ClientGet.Method, ClientGet.Handle);

app.MapMethods(ProductGetAll.Template, ProductGetAll.Method, ProductGetAll.Handle);
app.MapMethods(ProductPost.Template, ProductPost.Method, ProductPost.Handle);
app.MapMethods(ProductGetById.Template, ProductGetById.Method, ProductGetById.Handle);
app.MapMethods(ProductGetShowCase.Template, ProductGetShowCase.Method, ProductGetShowCase.Handle);

app.MapMethods(OrderPost.Template, OrderPost.Method, OrderPost.Handle);
app.MapMethods(OrderById.Template, OrderById.Method, OrderById.Handle);

app.MapMethods(TokenPost.Template, TokenPost.Method, TokenPost.Handle);

app.UseExceptionHandler("/error");
app.Map("/error", (HttpContext http) =>
{

    var error = http.Features?.Get<IExceptionHandlerFeature>()?.Error;

    if (error != null)
    {
        if (error is SqlException)
            return Results.Problem(title: "Database out", statusCode: 500);
        else if (error is BadHttpRequestException)
            return Results.Problem(title: "Error to convert data to other type. See all the information sent", statusCode: 500);
    }

    return Results.Problem(title: "An error ocurred", statusCode: 500);
});

app.Run();
