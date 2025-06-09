using System.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// Dependency injections
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<SqlConnection>(a =>
    new SqlConnection(builder.Configuration.GetConnectionString("ApplicationDB")));

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
