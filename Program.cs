using Microsoft.EntityFrameworkCore;
using DotNetCoreSqlDb.Data;
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

// Register services
builder.Services.AddSingleton<AzureStorageService>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    string connectionString = configuration["DefaultEndpointsProtocol=https;AccountName=acewwwroot;AccountKey=rANrk8t68qtk5ooKS4T+gRHGYHGdpFZRGUEz+iEWdVL5l3dwGgo8tAtSsxWPTGLBYqg9/Iz1g3L/+AStRxhgRw==;BlobEndpoint=https://acewwwroot.blob.core.windows.net/;FileEndpoint=https://acewwwroot.file.core.windows.net/;TableEndpoint=https://acewwwroot.table.core.windows.net/;QueueEndpoint=https://acewwwroot.queue.core.windows.net/"];
    return new AzureStorageService(connectionString);
});

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
