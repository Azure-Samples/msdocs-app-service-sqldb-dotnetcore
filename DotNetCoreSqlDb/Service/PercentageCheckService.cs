
using DotNetCoreSqlDb.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DotNetCoreSqlDb.Service;

public sealed class PercentageCheckService : BackgroundService
{
    private readonly CoreDbContext _context;

    public PercentageCheckService(CoreDbContext context)
    {
        _context = context;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        int delay = 2000;        

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                TimeSpan time = Help.GetEstDatetime().TimeOfDay;
                if (time > new TimeSpan(09, 31, 30) && time < new TimeSpan(15, 59, 55))
                {                   
                    var eTrade = new ETrade();
                    var optionDate = Help.GetEstDatetime().ToShortDateString();
                    eTrade.PercentageCheck(_context, optionDate);

                    Console.WriteLine($"PercentageCheck job - {Help.GetEstDatetime()}" );
                }
                await Task.Delay(delay, stoppingToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }    
}
