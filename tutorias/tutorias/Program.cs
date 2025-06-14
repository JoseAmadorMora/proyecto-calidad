using System.Data;
using System.Data.SqlClient;
using tutorias.Backend.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Dependency injections
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IDbConnection, SqlConnection>(a =>
    new SqlConnection(builder.Configuration.GetConnectionString("ApplicationDB")));
builder.Services.AddScoped<AuthenticationLogic>();
builder.Services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Authentication}/{action=LoginPage}/{id?}");

app.Run();
