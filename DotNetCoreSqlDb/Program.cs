using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DotNetCoreSqlDb.Data;
var builder = WebApplication.CreateBuilder(args);




string connectionString = "Server=todoiiot-server.postgres.database.azure.com;Database=todoiiot-database;Port=5432;Ssl Mode=Require;User Id=dcaigpqaja;Password=7337L4X67TVUO6F3$;";

// Add database context and cache
builder.Services.AddDbContext<MyDatabaseContext>(options =>

    options.UseSqlServer(connectionString));


builder.Services.AddStackExchangeRedisCache(options =>
{
options.Configuration = builder.Configuration["AZURE_REDIS_CONNECTIONSTRING"];
options.InstanceName = "SampleInstance";
});







// Add services to the container.
builder.Services.AddControllersWithViews();

// Add App Service logging
builder.Logging.AddAzureWebAppDiagnostics();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    // app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Todos}/{action=Index}/{id?}");

app.Run();
