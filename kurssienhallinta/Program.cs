using Microsoft.EntityFrameworkCore;
using Npgsql;
using kurssienhallinta.Models;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

Env.Load();
builder.Configuration.AddEnvironmentVariables();
var connectionString = Environment.GetEnvironmentVariable("Connection__Norsu") ?? builder.Configuration.GetConnectionString("DefaultConnection");


try
{
    using var conn = new Npgsql.NpgsqlConnection(connectionString);
    conn.Open();
    Console.WriteLine("Connected successfully! 🎉");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ {ex.GetType().Name}: {ex.Message}");
}

// Console.WriteLine($"DB: {connectionString}");

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
