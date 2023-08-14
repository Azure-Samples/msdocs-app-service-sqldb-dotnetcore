
using DotNetCoreSqlDb.Models;
using Microsoft.EntityFrameworkCore;


namespace DotNetCoreSqlDb.Data
{
    public class User1111Context : DbContext
    { 
        public DbSet<User> User { get; set; }
        public DbSet<Account> Account { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
              @"Data Source=naren-core-sql-server.database.windows.net,1433;Initial Catalog=naren-core-sql-database;User ID=naren-core-sql-server-admin;Password=Z2U6JGYZ3W448Y40$"
            );
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(b => b.UserEmail)
                .IsRequired();
        }
    }
}