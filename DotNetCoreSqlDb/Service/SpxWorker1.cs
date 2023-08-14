
using DotNetCoreSqlDb.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DotNetCoreSqlDb.Service;

public sealed class SpxWorker1 : BackgroundService
{
    private readonly ILogger<SpxWorker1> _logger;
    private readonly CoreDbContext _context;

    public SpxWorker1(ILogger<SpxWorker1> logger, CoreDbContext context)
    {
        _context = context;
        _logger = logger;
    }   


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var eTrade = new ETrade();

        while (!stoppingToken.IsCancellationRequested)
        {
            int delay = 2000;
            var users = _context.User.ToList();

            TimeSpan time = DateTime.Now.ToLocalTime().TimeOfDay;
            if (users != null && time > new TimeSpan(9, 30, 25) && time < new TimeSpan(15, 59, 40))
            {
                int intMinutes = (int)time.TotalMinutes % 15;
                if (time > new TimeSpan(15, 59, 20)) intMinutes = 0;
                
                delay = 10000;
              

                _logger.LogInformation($"Running INSIDE the window at: {DateTimeOffset.Now.ToLocalTime()}");
            }
            else if (users != null)
            {
               
                DateTimeOffset now = (DateTimeOffset)DateTime.Now.ToLocalTime();
                var heathCheckResponse = eTrade.HealthCheck(users.First(), "SPY", now);
                _logger.LogInformation($"Running OUTSIDE the window: {heathCheckResponse}");
            }

            //_logger.LogInformation($"{this.GetType()} running at: {DateTimeOffset.Now.ToLocalTime()}");
            await Task.Delay(delay, stoppingToken);
        }
    }
}
