using Microsoft.EntityFrameworkCore;
using DotNetCoreSqlDb.Data;
using StackExchange.Redis;
using System.Security.Authentication;

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
            var config = StackExchange.Redis.ConfigurationOptions.Parse(redisConnectionString);

            config.User = "default";

            options.ConfigurationOptions = config;
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


// =====================================================
// 🔴 REDIS RAW CONNECT TEST – ENDA TILLÄGGET
// =====================================================
app.Lifetime.ApplicationStarted.Register(async () =>
{
    var logger = app.Services
        .GetRequiredService<ILoggerFactory>()
        .CreateLogger("REDIS-RAW-TEST");

    try
    {
        var cs = Environment.GetEnvironmentVariable("AZURE_REDIS_CONNECTIONSTRING");

        logger.LogWarning(
            "REDIS RAW TEST | exists={exists} | length={len}",
            !string.IsNullOrWhiteSpace(cs),
            cs?.Length ?? 0
        );

        var options = ConfigurationOptions.Parse(cs);
        options.AbortOnConnectFail = false;
        options.ConnectTimeout = 15000;
        options.SyncTimeout = 15000;
        options.Ssl = true;
        options.SslProtocols = SslProtocols.Tls12;

        using var mux = await ConnectionMultiplexer.ConnectAsync(options);
        var db = mux.GetDatabase();

        await db.StringSetAsync("redis-raw-test", "ok");
        var value = await db.StringGetAsync("redis-raw-test");

        logger.LogWarning("REDIS RAW TEST SUCCESS | value={value}", value);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "REDIS RAW TEST FAILED");
    }
});
// =====================================================
// 🔴 SLUT PÅ TILLÄGG
// =====================================================


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
