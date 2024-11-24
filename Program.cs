using Microsoft.EntityFrameworkCore;
using DotNetCoreSqlDb.Data;
using Microsoft.Extensions.DependencyInjection;
var builder = WebApplication.CreateBuilder(args);

// Add database context and cache
if(builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<MyDatabaseContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("MyDbConnection")));
    builder.Services.AddDistributedMemoryCache();
}
 else
 {
     builder.Services.AddDbContext<MyDatabaseContext>(options =>
         options.UseSqlServer(builder.Configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING")));
     builder.Services.AddStackExchangeRedisCache(options =>
     {
     options.Configuration = builder.Configuration["AZURE_REDIS_CONNECTIONSTRING"];
     options.InstanceName = "SampleInstance";
     });
 }

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add App Service logging
builder.Logging.AddAzureWebAppDiagnostics();

builder.Services.AddSingleton<AzureStorageService>(provider =>
{
    // Retrieve IConfiguration from the DI container
    var configuration = provider.GetRequiredService<IConfiguration>();
    
    // Get the connection string from IConfiguration
    var connectionString = configuration.GetConnectionString("AzureStorage");
    
    // Check if connection string is null or empty
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new Exception("Azure Storage connection string is not configured properly.");
    }

    // Create and return the AzureStorageService with the connection string
    return new AzureStorageService(configuration);  // Passing IConfiguration as required by the constructor
});


// Add other services as needed
builder.Services.AddControllersWithViews(); // This is for MVC controllers

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Todos}/{action=Index}/{id?}");

app.Run();
