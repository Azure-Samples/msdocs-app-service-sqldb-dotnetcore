using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DotNetCoreSqlDb.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web.UI;

var builder = WebApplication.CreateBuilder(args);

// Add database context
builder.Services.AddDbContext<MyDatabaseContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING")));

// Conditionally set up caching based on the environment
if (builder.Environment.IsDevelopment())
{
    // Use in-memory cache for local development
    builder.Services.AddDistributedMemoryCache();
}
else
{
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = builder.Configuration["AZURE_REDIS_CONNECTIONSTRING"];
        options.InstanceName = "SampleInstance";
    });
}

// Add authentication and configure it for Azure AD B2C
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));

// Add controllers with views and require authenticated user
builder.Services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});
builder.Services.AddRazorPages().AddMicrosoftIdentityUI();

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
app.UseAuthentication();  // Add this line to enable authentication
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=ElectricalTestResults}/{action=Index}/{id?}");
    endpoints.MapRazorPages();
});

app.Run();