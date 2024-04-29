using DotNetCoreSqlDb.Models;
using Microsoft.EntityFrameworkCore;

namespace DotNetCoreSqlDb.Data;

public class MyDatabaseContext : DbContext
{
    public MyDatabaseContext(DbContextOptions<MyDatabaseContext> options)
        : base(options)
    {
    }

    public DbSet<Todo> Todo { get; set; } = default!;
}