using Microsoft.EntityFrameworkCore;
using DotNetCoreSqlDb.Data;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<MyDatabaseContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("MyDbConnection")));

    builder.Services.AddDistributedMemoryCache();
}
else
{
    // ---------- SQL (MÅSTE FINNAS) ----------
    var sqlConnectionString =
        builder.Configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING")
        ?? builder.Configuration["AZURE_SQL_CONNECTIONSTRING"];

    builder.Services.AddDbContext<MyDatabaseContext>(options =>
        options.UseSqlServer(sqlConnectionString));

    // ---------- REDIS (VALFRI) ----------
    var redisConnectionString =
        builder.Configuration.GetConnectionString("AZURE_REDIS_CONNECTIONSTRING")
        ?? builder.Configuration["AZURE_REDIS_CONNECTIONSTRING"];

    if (!string.IsNullOrWhiteSpace(redisConnectionString))
    {
        builder.Services.AddStackExchangeRedisCache(options =>
 {
     options.ConfigurationOptions =
         StackExchange.Redis.ConfigurationOptions.Parse(redisConnectionString);

     options.ConfigurationOptions.ConnectTimeout = 3000;
     options.ConfigurationOptions.SyncTimeout = 3000;
     options.ConfigurationOptions.AbortOnConnectFail = false;


     options.InstanceName = "SampleInstance";
 });

    }
    else
    {
        builder.Services.AddDistributedMemoryCache();
    }
}


// MVC
builder.Services.AddControllersWithViews();
builder.Logging.AddAzureWebAppDiagnostics();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MyDatabaseContext>();
    db.Database.Migrate();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Todos}/{action=Index}/{id?}");

app.Run();
