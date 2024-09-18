using Microsoft.EntityFrameworkCore;
using DotNetCoreSqlDb.Data;
using DotNetCoreSqlDb.Models.WagerItems;

namespace DotNetCoreSqlDb.WagerItems
{
    public static class SeedWagerData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new MyDatabaseContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<MyDatabaseContext>>()))
            {
                // Look for any movies.
                if (context.WagerItem.Any())
                {
                    return;   // DB has been seeded
                }
                context.WagerItem.AddRange(
                    new WagerItem
                    {
                        UserId = 1,
                        MatchId = 13210,
                        SportType = Types.SportTypes.NFL,
                        WagerValue = 100.00,
                        BetType = Types.BetTypes.Spread,
                        Odds = -115,
                        Active = true,
                        Outcome = null,
                        TimeOfBet = DateTime.Now
                    },
                    new WagerItem
                    {
                        UserId = 1,
                        MatchId = 13211,
                        SportType = Types.SportTypes.NFL,
                        WagerValue = 100.00,
                        BetType = Types.BetTypes.Spread,
                        Odds = -115,
                        Active = true,
                        Outcome = null,
                        TimeOfBet = DateTime.Now
                    },
                    new WagerItem
                    {
                        UserId = 1,
                        MatchId = 13212,
                        SportType = Types.SportTypes.NFL,
                        WagerValue = 100.00,
                        BetType = Types.BetTypes.Spread,
                        Odds = -115,
                        Active = true,
                        Outcome = null,
                        TimeOfBet = DateTime.Now
                    }
                );
                context.SaveChanges();
            }
        }
    }
}
