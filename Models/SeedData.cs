using Microsoft.EntityFrameworkCore;
using DotNetCoreSqlDb.Data;

namespace DotNetCoreSqlDb.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new MyDatabaseContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<MyDatabaseContext>>()))
            {
                // Look for any movies.
                if (context.Degenerate.Any())
                {
                    return;   // DB has been seeded
                }
                context.Degenerate.AddRange(
                    new Degenerate
                    {
                        Username = "imhosu",
                        CashWallet = 87.05,
                        BetsPlaced = 0,
                        BetsWon = 0,
                        TotalWagesPlaced = 0,
                        TotalWagesWon = 0,
                    },
                    new Degenerate
                    {
                        Username = "harold",
                        CashWallet = 87.05,
                        BetsPlaced = 0,
                        BetsWon = 0,
                        TotalWagesPlaced = 0,
                        TotalWagesWon = 0,
                    },
                    new Degenerate
                    {
                        Username = "gene",
                        CashWallet = 30.00,
                        BetsPlaced = 0,
                        BetsWon = 0,
                        TotalWagesPlaced = 0,
                        TotalWagesWon = 0,
                    },
                    new Degenerate
                    {
                        Username = "mom",
                        CashWallet = 30.00,
                        BetsPlaced = 0,
                        BetsWon = 0,
                        TotalWagesPlaced = 0,
                        TotalWagesWon = 0,
                    }
                );
                context.SaveChanges();
            }
        }
    }
}
