using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DotNetCoreSqlDb.Data;
using DotNetCoreSqlDb.Models;
using DotNetCoreSqlDb.Service;

var builder = WebApplication.CreateBuilder(args);

// Add database context and cache
builder.Services.AddDbContext<CoreDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyDbConnection")));
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["AZURE_REDIS_CONNECTIONSTRING"];
    options.InstanceName = "SampleInstance";
});

// Add services to the container.
builder.Services.AddControllersWithViews();

var optionsBuilder = new DbContextOptionsBuilder<CoreDbContext>();
optionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString("MyDbConnection"));

builder.Services.AddSingleton<IHostedService>(serviceProvider => new HealtherService(new CoreDbContext(optionsBuilder.Options)));
builder.Services.AddSingleton<IHostedService>(serviceProvider => new PercentageCheckService(new CoreDbContext(optionsBuilder.Options)));
builder.Services.AddSingleton<IHostedService>(serviceProvider => new CloseAllEoDService(new CoreDbContext(optionsBuilder.Options)));


// Add App Service logging
builder.Logging.AddAzureWebAppDiagnostics();
builder.Services.AddSwaggerGen();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseExceptionHandler("/Home/Error");
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=EMASignalMvc}/{action=Index}/{id?}");

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});

app.UseSwagger(options =>
{
    options.SerializeAsV2 = true;
});

app.Run();


