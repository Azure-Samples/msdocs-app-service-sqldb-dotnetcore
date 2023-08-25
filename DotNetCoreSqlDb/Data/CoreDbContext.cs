using Microsoft.EntityFrameworkCore;
using DotNetCoreSqlDb.Models;

namespace DotNetCoreSqlDb.Data
{
    public class CoreDbContext : DbContext
    {
        public CoreDbContext(DbContextOptions<CoreDbContext> options)
            : base(options)
        {
        }       

        public DbSet<User> User { get; set; }
        public DbSet<Account> Account { get; set; }

        public DbSet<TVSignal> TVSignal { get; set; } = default!;

        public DbSet<OptionBase> Option { get; set; }

        #region Required
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(b => b.UserEmail)
                .IsRequired();
        }
        #endregion

        #region Required
        //public DbSet<DotNetCoreSqlDb.Models.OrderBookDemo> OrderBookDemo { get; set; } = default!;
        #endregion

        #region Required
        public DbSet<DotNetCoreSqlDb.Models.OrderBookOptionSelling> OrderBookOptionSelling { get; set; } = default!;
        #endregion

        #region Required
        public DbSet<DotNetCoreSqlDb.Models.OrderBookOptionBuying> OrderBookOptionBuying { get; set; } = default!;
        #endregion

        #region Required
        public DbSet<DotNetCoreSqlDb.Models.OptionBuyingPnLTracker> OptionBuyingPnLTracker { get; set; } = default!;
        #endregion

        #region Required
        public DbSet<DotNetCoreSqlDb.Models.OptionSellingPnLTracker> OptionSellingPnLTracker { get; set; } = default!;
        #endregion
    }
}