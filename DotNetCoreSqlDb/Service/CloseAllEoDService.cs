
using DotNetCoreSqlDb.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DotNetCoreSqlDb.Service;

public sealed class CloseAllEoDService : BackgroundService
{
    private readonly CoreDbContext _context;

    public CloseAllEoDService(CoreDbContext context)
    {
        _context = context;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        int delay = 1000 * 60 * 5;        

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                TimeSpan time = Help.GetEstDatetime().TimeOfDay;
                var eTrade = new ETrade();

                if (time > new TimeSpan(15, 58, 30) && time < new TimeSpan(15, 59, 55))
                {
                    var optionDate = Help.GetEstDatetime().ToShortDateString();
                    eTrade.CloseAll(_context, optionDate);

                    Console.WriteLine($"Closing all orders by EOD job - {Help.GetEstDatetime()}");
                }

                if (time > new TimeSpan(15, 50, 00) && time < new TimeSpan(15, 59, 59))
                {
                    delay = 500;
                }
                else
                {
                    delay = 1000 * 60 * 5;
                    Console.WriteLine($"CloseAllEoDService is ON. All orders closes after 03:58:30 PM EST - {Help.GetEstDatetime()}");
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
