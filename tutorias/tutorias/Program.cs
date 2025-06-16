using System.Data;
using System.Data.SqlClient;
using tutorias.Backend.Authentication;
using tutorias.Backend.Tutoring;

var builder = WebApplication.CreateBuilder(args);

// Dependency injections
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IDbConnection, SqlConnection>(a =>
    new SqlConnection(builder.Configuration.GetConnectionString("ApplicationDB")));

builder.Services.AddScoped<AuthenticationLogic>();
builder.Services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();

builder.Services.AddScoped<TutoringLogic>();
builder.Services.AddScoped<ITutoringRepository, TutoringRepository>();

builder.Services.AddSession();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Authentication}/{action=LoginPage}/{id?}");

app.Run();
