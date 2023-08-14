
using DotNetCoreSqlDb.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DotNetCoreSqlDb.Service;

public sealed class HealtherService : BackgroundService
{    
    private readonly CoreDbContext _context;

    public HealtherService(CoreDbContext context)
    {
        _context = context;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var eTrade = new ETrade();

        while (!stoppingToken.IsCancellationRequested)
        {
            int delay =30000;
            var users = _context.User.ToList();

            DateTimeOffset now = (DateTimeOffset)Help.GetEstDatetime();
            var heathCheckResponse = eTrade.HealthCheck(users.First(), "SPY", now);
            Console.WriteLine($"Heathcheck: {heathCheckResponse}");

            await Task.Delay(delay, stoppingToken);
        }
    }
}
