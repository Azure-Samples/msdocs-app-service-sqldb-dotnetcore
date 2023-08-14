
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
                    var users = _context.User.ToList();
                    DateTimeOffset now = (DateTimeOffset)Help.GetEstDatetime();
                    var batchId = now.ToUnixTimeSeconds();
                    var batchDateTime = now.ToString("yyyy-MM-dd hh:mm:ss.fff tt");

                    var callOrderBookOptionBuying = _context.OrderBookOptionBuying.ToList().Where(a => a.closeBatch <= 0).ToList();
                    foreach (var order in callOrderBookOptionBuying)
                    {
                        var option = eTrade.GetOptionDetails(_context, users.First(), order.optionDate, order.symbol, order.strikePrice, order.strikePrice, order.optionType, batchId, batchDateTime);
                        eTrade.CloseOrderOptionBuying(_context, order, option, order.id);
                    }

                    var callOrderBookOptionSelling = _context.OrderBookOptionSelling.ToList().Where(a => a.closeBatch <= 0).ToList();
                    foreach (var order in callOrderBookOptionSelling)
                    {
                        var option = eTrade.GetOptionDetails(_context, users.First(), order.optionDate, order.symbol, order.strikePrice, order.strikePrice, order.optionType, batchId, batchDateTime);
                        eTrade.CloseOrderOptionSelling(_context, order, option, order.id);
                    }

                    Console.WriteLine($"All Orders Closed");
                }

                if (time > new TimeSpan(15, 50, 00) && time < new TimeSpan(15, 59, 59))
                {
                    delay = 500;
                }
                else
                {
                    delay = 1000 * 60 * 5;
                    Console.WriteLine($"CloseAllEoDService is ON. All orders closes after 03:58:30 PM EST ");
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
