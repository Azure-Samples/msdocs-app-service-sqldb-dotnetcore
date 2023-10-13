using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DotNetCoreSqlDb.Data;
using Microsoft.Data.SqlClient;
using Azure.Identity;

 
var builder = WebApplication.CreateBuilder(args);

// Add database context and cache
string azure_connection_string = String.Empty;
if (builder.Environment.IsDevelopment())
{
    // get from appsettings.json
    azure_connection_string = builder.Configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING");
}
else
{
    // get from Environment
    azure_connection_string = Environment.GetEnvironmentVariable("AZURE_SQL_CONNECTIONSTRING");
}
// Uncomment one of the two lines depending on the identity type    
SqlConnection authenticatedConnection = new SqlConnection(azure_connection_string); // system-assigned identity
//SqlConnection authenticatedConnection = new SqlConnection("Server=tcp:<server-name>.database.windows.net;Database=<database-name>;Authentication=Active Directory Default;User Id=<client-id-of-user-assigned-identity>;TrustServerCertificate=True"); // user-assigned identity

// get the database context
builder.Services.AddDbContext<MyDatabaseContext>(options =>
    options.UseSqlServer(authenticatedConnection.ConnectionString));


//builder.Services.AddDbContext<MyDatabaseContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING")));

builder.Services.AddDistributedMemoryCache();

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
