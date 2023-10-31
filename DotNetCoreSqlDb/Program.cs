using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore.SqlServer;
using DotNetCoreSqlDb.Data;
using System.Diagnostics;
using Npgsql;
using System.Runtime.Intrinsics.X86;


var builder = WebApplication.CreateBuilder(args);


//builder.Services.AddSqlServer<MyDatabaseContext>(builder.Configuration.GetConnectionString("AZURE_POSTGRESQL_CONNECTIONSTRING"));

/*
// Add database context and cache
builder.Services.AddDbContext<MyDatabaseContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AZURE_POSTGRESQL_CONNECTIONSTRING")));
*/


// Best practice is to scope the NpgsqlConnection to a "using" block
using (NpgsqlConnection conn = new NpgsqlConnection(builder.Configuration.GetConnectionString("AZURE_POSTGRESQL_CONNECTIONSTRING")))
{
    // Connect to the database
    conn.Open();


}






builder.Services.AddStackExchangeRedisCache(options =>
{
options.Configuration = builder.Configuration.GetConnectionString("AZURE_REDIS_CONNECTIONSTRING");
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
