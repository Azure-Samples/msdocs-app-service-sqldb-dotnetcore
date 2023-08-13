using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DotNetCoreSqlDb.Data;
using DotNetCoreSqlDb.Models;

var builder = WebApplication.CreateBuilder(args);

// Add database context and cache
builder.Services.AddDbContext<TodoDb>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyDbConnection")));
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

app.MapGet("/todoitems", async (TodoDb db) =>
    await db.Todo.ToListAsync());



app.MapGet("/todoitems/{id}", async (int id, TodoDb db) =>
    await db.Todo.FindAsync(id)
        is Todo todo
            ? Results.Ok(todo)
            : Results.NotFound());
app.MapPost("/todoitems", async (Todo todo, TodoDb db) =>
{
    db.Todo.Add(todo);
    await db.SaveChangesAsync();

    return Results.Created($"/todoitems/{todo.ID}", todo);
});

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
