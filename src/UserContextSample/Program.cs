using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using UserContextSample;
using UserContextSample.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<TodosService>();
if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Test")
    builder.Services.AddScoped<IContextResolver, HttpContextResolver>();
else
    builder.Services.AddScoped<IContextResolver, TestHttpContextResolver>();

builder.Services.AddScoped<UserContext>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Settings.SecretKey)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/login", () =>
{
    return Results.Ok(new
    {
        Token = TokenService.GenerateToken(
            new(
                "Arthur",
                Guid.Parse("A435542D-402C-459E-AB37-DE2C5109320E")
            )
        )
    });
});

app.MapPut("todos/close", (ClaimsPrincipal user, TodosService service) =>
{
    var departmentId = Guid.Parse(user.FindFirstValue(CustomClaims.DepartmentId));
    service.CloseAllByDepartment(departmentId);

}).RequireAuthorization();

app.MapGet("todos/closed", (ClaimsPrincipal user, TodosService service) =>
{
    var departmentId = Guid.Parse(user.FindFirstValue(CustomClaims.DepartmentId));
    return Results.Ok(service.GetAllClosedByDepartment(departmentId));

}).RequireAuthorization();

//app.MapPut("todos/close", (TodosService service) => service.CloseAllByDepartment()).RequireAuthorization();

//app.MapGet("todos/closed", (TodosService service) => Results.Ok(service.GetAllClosedByDepartment())).RequireAuthorization();

app.Run();
