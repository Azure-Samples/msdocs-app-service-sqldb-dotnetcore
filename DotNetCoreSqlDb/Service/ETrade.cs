using Azure.Core;
using DotNetCoreSqlDb.Data;
using DotNetCoreSqlDb.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serializers;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Web;

namespace DotNetCoreSqlDb.Service;

public class ETrade
{

    public Option GetOptionDetails(CoreDbContext context, User user, string optionDate, string symbol, double price, double strikePriceNear, string chainType, long batchId, string batchDateTime, bool saveOption = true)
    {
        DateTime optionDateLocal = DateTime.Parse(optionDate, new CultureInfo("en-US", true));

        int expiryYear = optionDateLocal.Year;
        int expiryMonth = optionDateLocal.Month;
        int expiryDay = optionDateLocal.Day;
        int noOfStrikes = 1;

        List<Option> options = new List<Option>();
        JObject optionJson = new JObject();
        try
        {
            string etradeBaseUrl = user.EtradeBaseUrl;
            var client = new RestClient(etradeBaseUrl);

            symbol = symbol.Equals("SPXW") ? "SPX" : symbol;

            var quoteRequest = new RestRequest($"v1/market/optionchains?symbol={symbol}&expiryYear={expiryYear}&expiryMonth={expiryMonth}&expiryDay={expiryDay}&includeWeekly={false}&chainType={chainType}&strikePriceNear={strikePriceNear}&noOfStrikes={noOfStrikes}");
            client.Authenticator = OAuth1Authenticator.ForProtectedResource(user.ConsumerKey, user.ConsumerSecret, user.AccessToken, user.AccessTokenSecret);
            var response = client.Execute(quoteRequest);

            optionJson = JObject.Parse(response.Content);
            options = BuildOptionBase(batchId, batchDateTime, price, optionJson.ToString());

            if (saveOption)
            {
                context.Option.AddRange(options);
                context.SaveChanges();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return options.First();
    }

    
    public void OpenOrderOptionBuying(CoreDbContext context, Option option, int singalId)
    {

        var orderBookOptionBuying = new OrderBookOptionBuying
        {
            symbol = option.Symbol,
            optionType = option.OptionType,
            optionDate = option.OptionDate,
            strikePrice = option.StrikePrice,
            openBatch = option.Batch,
            openBatchDateTime = option.BatchDateTime,
            openSinalId = singalId,
            openStokePrice = option.Price,
            openCost = option.Ask,
            openDateTime = Help.GetEstDatetime(),
            closeBatchDateTime = option.BatchDateTime,
            //=========================================
            percentFlag1 = "@10%",
            expectedPnL1 = Math.Round(option.Price * 0.1, 3),
            actualPnL1 = 0,
            batch1 = 0,
            percentFlag2 = "@20%",
            expectedPnL2 = Math.Round(option.Price * 0.2, 3),
            actualPnL2 = 0,
            batch2 = 0,
            percentFlag3 = "@30%",
            expectedPnL3 = Math.Round(option.Price * 0.3, 3),
            actualPnL3 = 0,
            batch3 = 0,
            percentFlag4 = "@40%",
            expectedPnL4 = Math.Round(option.Price * 0.4, 3),
            actualPnL4 = 0,
            batch4 = 0,
            percentFlag5 = "@50%",
            expectedPnL5 = Math.Round(option.Price * 0.5, 3),
            actualPnL5 = 0,
            batch5 = 0
        };

        context.Add(orderBookOptionBuying);
        context.SaveChanges();
    }

    public void CloseOrderOptionBuying(CoreDbContext context, OrderBookOptionBuying orderBookOptionBuying, Option option, int singalId)
    {
        orderBookOptionBuying.closeBatch = option.Batch;
        orderBookOptionBuying.closeBatchDateTime = option.BatchDateTime;
        orderBookOptionBuying.closeDateTime = Help.GetEstDatetime();
        orderBookOptionBuying.closeSinalId = singalId;
        orderBookOptionBuying.closeStokePrice = option.Price;
        orderBookOptionBuying.closeCost = option.Bid;
        orderBookOptionBuying.PnL = Math.Round(orderBookOptionBuying.closeCost - orderBookOptionBuying.openCost, 3);
        if(orderBookOptionBuying.openCost > 0) orderBookOptionBuying.PnLPercentage = (orderBookOptionBuying.PnL / orderBookOptionBuying.openCost).ToString("0.00%");

        context.Update(orderBookOptionBuying);
        context.SaveChanges();
    }

    public void OpenOrderOptionSelling(CoreDbContext context, Option option, int singalId)
    {
        var orderBookOptionSelling = new OrderBookOptionSelling
        {
            symbol = option.Symbol,
            optionType = option.OptionType,
            optionDate = option.OptionDate,
            strikePrice = option.StrikePrice,
            openBatch = option.Batch,
            openBatchDateTime = option.BatchDateTime,
            openSinalId = singalId,
            openStokePrice = option.Price,
            openCost = option.Bid,
            openDateTime = Help.GetEstDatetime(),
            closeBatchDateTime = option.BatchDateTime,
            //=========================================
            percentFlag1 = "@10%",
            expectedPnL1 = Math.Round(option.Price * 0.1, 3),
            actualPnL1 = 0,
            batch1 = 0,
            percentFlag2 = "@20%",
            expectedPnL2 = Math.Round(option.Price * 0.2, 3),
            actualPnL2 = 0,
            batch2 = 0,
            percentFlag3 = "@30%",
            expectedPnL3 = Math.Round(option.Price * 0.3, 3),
            actualPnL3 = 0,
            batch3 = 0,
            percentFlag4 = "@40%",
            expectedPnL4 = Math.Round(option.Price * 0.4, 3),
            actualPnL4 = 0,
            batch4 = 0,
            percentFlag5 = "@50%",
            expectedPnL5 = Math.Round(option.Price * 0.5, 3),
            actualPnL5 = 0,
            batch5 = 0
        };

        context.Add(orderBookOptionSelling);
        context.SaveChanges();
    }

    public void CloseOrderOptionSelling(CoreDbContext context, OrderBookOptionSelling orderBookOptionSelling, Option option, int singalId)
    {
        orderBookOptionSelling.closeBatch = option.Batch;
        orderBookOptionSelling.closeBatchDateTime = option.BatchDateTime;
        orderBookOptionSelling.closeDateTime = Help.GetEstDatetime();
        orderBookOptionSelling.closeSinalId = singalId;
        orderBookOptionSelling.closeStokePrice = option.Price;
        orderBookOptionSelling.closeCost = option.Ask;
        orderBookOptionSelling.PnL = Math.Round(orderBookOptionSelling.openCost - orderBookOptionSelling.closeCost, 3);
        if (orderBookOptionSelling.openCost > 0) orderBookOptionSelling.PnLPercentage = (orderBookOptionSelling.PnL / orderBookOptionSelling.openCost).ToString("0.00%");

        context.Update(orderBookOptionSelling);
        context.SaveChanges();
    }

    public List<Option> BuildOptionBase(long timeStamp, string batchDateTime, double price, string optionJson)
    {
        var optionChain = JsonConvert.DeserializeObject<OptionChain>(optionJson);

        List<Option> calls = new List<Option>();
        foreach (var item in optionChain.OptionChainResponse.OptionPair)
        {
            if (item.Call != null && item.Call.OptionRootSymbol != "SPX")
            {
                item.Call.Batch = timeStamp;
                item.Call.BatchDateTime = batchDateTime;
                item.Call.Price = price;
                calls.Add((Option)item.Call);
            }

            if (item.Put != null && item.Put.OptionRootSymbol != "SPX")
            {
                item.Put.Batch = timeStamp;
                item.Put.BatchDateTime = batchDateTime;
                item.Put.Price = price;
                calls.Add((Option)item.Put);
            }
        }
        foreach (var item in calls)
        {
            item.Symbol = item.OptionRootSymbol;
            item.Rho = item.Rho;
            item.Vega = item.Vega;
            item.Theta = item.Theta;
            item.Delta = item.Delta;
            item.Gamma = item.Gamma;
            item.Iv = item.Iv;


            item.CurrentValue = item.CurrentValue;

            item.OptionPrice = item.OptionPrice;
            item.OptionDate = item.OptionDate;
            item.MinutesToExpire = item.MinutesToExpire;
        }
        return calls;

    }

    public string HealthCheck(CoreDbContext context, string symbol, DateTimeOffset now)
    {
        List<Option> options = new List<Option>();
        JObject optionJson = new JObject();
        try
        {
            var users = context.User.ToList();
            var user = users.FirstOrDefault();

            string etradeBaseUrl = user.EtradeBaseUrl;
            var client = new RestClient(etradeBaseUrl);

            var quoteRequest = new RestRequest($"v1/market/quote/{symbol}");
            client.Authenticator = OAuth1Authenticator.ForProtectedResource(user.ConsumerKey, user.ConsumerSecret, user.AccessToken, user.AccessTokenSecret);
            var response = client.Execute(quoteRequest);

            if (response.Content != null && response.Content.Contains("oauth_problem"))
                new Exception($"|****** OAUTH PROBLEM ******|");

            var stockQuote = JsonConvert.DeserializeObject<StockQuoteResponse>(response.Content.ToString());

            var price = 0.0;
            if (stockQuote != null && stockQuote.QuoteResponse.QuoteData.Count > 0)
                price = stockQuote.QuoteResponse.QuoteData[0].All.ask;

            return $"{symbol} : {price}";
        }
        catch (Exception ex)
        {
            return "Error:" + ex.Message;
        }
    }

    public void RunOrderBookOptionBuying(CoreDbContext _context, TVSignal tVSignal, string optionDate, int hoursLeft, string emaPeriodFactor)
    {
        DateTimeOffset now = (DateTimeOffset)Help.GetEstDatetime();
        var batchId = now.ToUnixTimeSeconds();
        var batchDateTime = now.ToString("yyyy-MM-dd hh:mm:ss.fff tt");
        double awayFactor = (tVSignal.Price * 0.001) + (hoursLeft * 1.5);
        if (tVSignal.Simbol.ToUpper().Equals("SPY")) awayFactor = ((10 * tVSignal.Price * 0.001) + (hoursLeft * 1.5)) * 0.1;

        ETrade etrade = new ETrade();
        var users = _context.User.ToList();

        if (tVSignal.Signal.ToUpper().Equals("LONG") && tVSignal.Period.Equals(emaPeriodFactor))
        {
            string closeOptionType = "PUT";
            string optionType = "CALL";

            var putOrderBook = _context.OrderBookOptionBuying.ToList().Where(a => a.symbol.Contains(tVSignal.Simbol) && Convert.ToDateTime(a.optionDate) == Convert.ToDateTime(optionDate) && a.optionType == closeOptionType && a.closeBatch <= 0).ToList();
            foreach (var order in putOrderBook)
            {
                var option = etrade.GetOptionDetails(_context, users[0], optionDate, tVSignal.Simbol, tVSignal.Price, order.strikePrice, order.optionType, batchId, batchDateTime);
                etrade.CloseOrderOptionBuying(_context, order, option, tVSignal.id);
            }
            TimeSpan time = Help.GetEstDatetime().TimeOfDay;
            if (time > new TimeSpan(09, 35, 00) && time < new TimeSpan(15, 55, 00))
            {
                double strikePriceNear = Math.Round(tVSignal.Price + awayFactor);
                var callOrderBook = _context.OrderBookOptionBuying.ToList().Where(a => a.symbol.Contains(tVSignal.Simbol) && Convert.ToDateTime(a.optionDate) == Convert.ToDateTime(optionDate) && a.optionType == optionType && a.closeBatch <= 0).ToList();
                if (callOrderBook.Count <= 0)
                {
                    var option = etrade.GetOptionDetails(_context, users[0], optionDate, tVSignal.Simbol, tVSignal.Price, Convert.ToInt64(strikePriceNear), optionType, batchId, batchDateTime);
                    etrade.OpenOrderOptionBuying(_context, option, tVSignal.id);
                }
            }
        }
        else if (tVSignal.Signal.ToUpper().Equals("SHORT") && tVSignal.Period.Equals(emaPeriodFactor))
        {
            string closeOptionType = "CALL";
            string optionType = "PUT";
            var callOrderBook = _context.OrderBookOptionBuying.ToList().Where(a => a.symbol.Contains(tVSignal.Simbol) && Convert.ToDateTime(a.optionDate) == Convert.ToDateTime(optionDate) && a.optionType == closeOptionType && a.closeBatch <= 0).ToList();

            foreach (var order in callOrderBook)
            {
                var option = etrade.GetOptionDetails(_context, users[0], optionDate, tVSignal.Simbol, tVSignal.Price, order.strikePrice, order.optionType, batchId, batchDateTime);
                etrade.CloseOrderOptionBuying(_context, order, option, tVSignal.id);
            }
            TimeSpan time = Help.GetEstDatetime().TimeOfDay;
            if (time > new TimeSpan(09, 35, 00) && time < new TimeSpan(15, 55, 00))
            {
                double strikePriceNear = Math.Round(tVSignal.Price - awayFactor);
                var putOrderBook = _context.OrderBookOptionBuying.ToList().Where(a => a.symbol.Contains(tVSignal.Simbol) && Convert.ToDateTime(a.optionDate) == Convert.ToDateTime(optionDate) && a.optionType == optionType && a.closeBatch <= 0).ToList();
                if (putOrderBook.Count <= 0)
                {
                    var option = etrade.GetOptionDetails(_context, users[0], optionDate, tVSignal.Simbol, tVSignal.Price, Convert.ToInt64(strikePriceNear), optionType, batchId, batchDateTime);
                    etrade.OpenOrderOptionBuying(_context, option, tVSignal.id);
                }
            }
        }
    }

    public void RunOrderBookOptionSelling(CoreDbContext _context, TVSignal tVSignal, string optionDate, int hoursLeft, string emaPeriodFactor)
    {
        DateTimeOffset now = (DateTimeOffset)Help.GetEstDatetime();
        var batchId = now.ToUnixTimeSeconds();
        var batchDateTime = now.ToString("yyyy-MM-dd hh:mm:ss.fff tt");
        double awayFactor = (tVSignal.Price * 0.00125) - hoursLeft;
        if (tVSignal.Simbol.ToUpper().Equals("SPY")) awayFactor = ((10 * tVSignal.Price * 0.00125) - hoursLeft) * 0.1;

        ETrade etrade = new ETrade();
        var users = _context.User.ToList();

        if (tVSignal.Signal.ToUpper().Equals("LONG") && tVSignal.Period.Equals(emaPeriodFactor))
        {
            string closeOptionType = "CALL";
            string optionType = "PUT";
            var callOrderBook = _context.OrderBookOptionSelling.ToList().Where(a => a.symbol.Contains(tVSignal.Simbol) && Convert.ToDateTime(a.optionDate) == Convert.ToDateTime(optionDate) && a.optionType == closeOptionType && a.closeBatch <= 0).ToList();

            foreach (var order in callOrderBook)
            {
                var option = etrade.GetOptionDetails(_context, users[0], optionDate, tVSignal.Simbol, tVSignal.Price, order.strikePrice, order.optionType, batchId, batchDateTime);
                etrade.CloseOrderOptionSelling(_context, order, option, tVSignal.id);
            }
            TimeSpan time = Help.GetEstDatetime().TimeOfDay;
            if (time > new TimeSpan(09, 35, 00) && time < new TimeSpan(15, 55, 00))
            {
                double strikePriceNear = Math.Round(tVSignal.Price - awayFactor);
                var putOrderBook = _context.OrderBookOptionSelling.ToList().Where(a => a.symbol.Contains(tVSignal.Simbol) && Convert.ToDateTime(a.optionDate) == Convert.ToDateTime(optionDate) && a.optionType == optionType && a.closeBatch <= 0).ToList();
                if (putOrderBook.Count <= 0)
                {
                    var option = etrade.GetOptionDetails(_context, users[0], optionDate, tVSignal.Simbol, tVSignal.Price, Convert.ToInt64(strikePriceNear), optionType, batchId, batchDateTime);
                    etrade.OpenOrderOptionSelling(_context, option, tVSignal.id);
                }
            }
        }

        if (tVSignal.Signal.ToUpper().Equals("SHORT") && tVSignal.Period.Equals(emaPeriodFactor))
        {
            string closeOptionType = "PUT";
            string optionType = "CALL";

            var putOrderBook = _context.OrderBookOptionSelling.ToList().Where(a => a.symbol.Contains(tVSignal.Simbol) && Convert.ToDateTime(a.optionDate) == Convert.ToDateTime(optionDate) && a.optionType == closeOptionType && a.closeBatch <= 0).ToList();
            foreach (var order in putOrderBook)
            {
                var option = etrade.GetOptionDetails(_context, users[0], optionDate, tVSignal.Simbol, tVSignal.Price, order.strikePrice, order.optionType, batchId, batchDateTime);
                etrade.CloseOrderOptionSelling(_context, order, option, tVSignal.id);
            }
            TimeSpan time = Help.GetEstDatetime().TimeOfDay;
            if (time > new TimeSpan(09, 35, 00) && time < new TimeSpan(15, 55, 00))
            {
                double strikePriceNear = Math.Round(tVSignal.Price + awayFactor);
                var callOrderBook = _context.OrderBookOptionSelling.ToList().Where(a => a.symbol.Contains(tVSignal.Simbol) && Convert.ToDateTime(a.optionDate) == Convert.ToDateTime(optionDate) && a.optionType == optionType && a.closeBatch <= 0).ToList();
                if (callOrderBook.Count <= 0)
                {
                    var option = etrade.GetOptionDetails(_context, users[0], optionDate, tVSignal.Simbol, tVSignal.Price, Convert.ToInt64(strikePriceNear), optionType, batchId, batchDateTime);
                    etrade.OpenOrderOptionSelling(_context, option, tVSignal.id);
                }
            }
        }
    }


    public void CloseAllBuyingOrders(CoreDbContext _context, TVSignal tVSignal, string optionDate, int hoursLeft, string emaPeriodFactor)
    {
        DateTimeOffset now = (DateTimeOffset)Help.GetEstDatetime();
        var batchId = now.ToUnixTimeSeconds();
        var batchDateTime = now.ToString("yyyy-MM-dd hh:mm:ss.fff tt");
        double awayFactor = (tVSignal.Price * 0.001) + (hoursLeft * 1.5);
        if (tVSignal.Simbol.ToUpper().Equals("SPY")) awayFactor = ((10 * tVSignal.Price * 0.001) + (hoursLeft * 1.5)) * 0.1;

        ETrade etrade = new ETrade();
        var users = _context.User.ToList();

        if (tVSignal.Signal.ToUpper().Equals("LONG") && tVSignal.Period.Equals(emaPeriodFactor))
        {
            string closeOptionType = "PUT";          

            var putOrderBook = _context.OrderBookOptionBuying.ToList().Where(a => a.symbol.Contains(tVSignal.Simbol) && Convert.ToDateTime(a.optionDate) == Convert.ToDateTime(optionDate) && a.optionType == closeOptionType && a.closeBatch <= 0).ToList();
            foreach (var order in putOrderBook)
            {
                var option = etrade.GetOptionDetails(_context, users[0], optionDate, tVSignal.Simbol, tVSignal.Price, order.strikePrice, order.optionType, batchId, batchDateTime);
                etrade.CloseOrderOptionBuying(_context, order, option, tVSignal.id);
            }
        }
        else if (tVSignal.Signal.ToUpper().Equals("SHORT") && tVSignal.Period.Equals(emaPeriodFactor))
        {
            string closeOptionType = "CALL";           
            var callOrderBook = _context.OrderBookOptionBuying.ToList().Where(a => a.symbol.Contains(tVSignal.Simbol) && Convert.ToDateTime(a.optionDate) == Convert.ToDateTime(optionDate) && a.optionType == closeOptionType && a.closeBatch <= 0).ToList();

            foreach (var order in callOrderBook)
            {
                var option = etrade.GetOptionDetails(_context, users[0], optionDate, tVSignal.Simbol, tVSignal.Price, order.strikePrice, order.optionType, batchId, batchDateTime);
                etrade.CloseOrderOptionBuying(_context, order, option, tVSignal.id);
            }
        }
    }

    public void CloseAllSellingOrders(CoreDbContext _context, TVSignal tVSignal, string optionDate, int hoursLeft, string emaPeriodFactor)
    {
        DateTimeOffset now = (DateTimeOffset)Help.GetEstDatetime();
        var batchId = now.ToUnixTimeSeconds();
        var batchDateTime = now.ToString("yyyy-MM-dd hh:mm:ss.fff tt");
        double awayFactor = (tVSignal.Price * 0.00125) - hoursLeft;
        if (tVSignal.Simbol.ToUpper().Equals("SPY")) awayFactor = ((10 * tVSignal.Price * 0.00125) - hoursLeft) * 0.1;

        ETrade etrade = new ETrade();
        var users = _context.User.ToList();

        if (tVSignal.Signal.ToUpper().Equals("LONG") && tVSignal.Period.Equals(emaPeriodFactor))
        {
            string closeOptionType = "CALL";
            var callOrderBook = _context.OrderBookOptionSelling.ToList().Where(a => a.symbol.Contains(tVSignal.Simbol) && Convert.ToDateTime(a.optionDate) == Convert.ToDateTime(optionDate) && a.optionType == closeOptionType && a.closeBatch <= 0).ToList();

            foreach (var order in callOrderBook)
            {
                var option = etrade.GetOptionDetails(_context, users[0], optionDate, tVSignal.Simbol, tVSignal.Price, order.strikePrice, order.optionType, batchId, batchDateTime);
                etrade.CloseOrderOptionSelling(_context, order, option, tVSignal.id);
            }
        }

        if (tVSignal.Signal.ToUpper().Equals("SHORT") && tVSignal.Period.Equals(emaPeriodFactor))
        {
            string closeOptionType = "PUT";

            var putOrderBook = _context.OrderBookOptionSelling.ToList().Where(a => a.symbol.Contains(tVSignal.Simbol) && Convert.ToDateTime(a.optionDate) == Convert.ToDateTime(optionDate) && a.optionType == closeOptionType && a.closeBatch <= 0).ToList();
            foreach (var order in putOrderBook)
            {
                var option = etrade.GetOptionDetails(_context, users[0], optionDate, tVSignal.Simbol, tVSignal.Price, order.strikePrice, order.optionType, batchId, batchDateTime);
                etrade.CloseOrderOptionSelling(_context, order, option, tVSignal.id);
            }
        }
    }

    public void CloseAll(CoreDbContext _context, string optionDate)
    {
        var users = _context.User.ToList();
        DateTimeOffset now = (DateTimeOffset)Help.GetEstDatetime();
        var batchId = now.ToUnixTimeSeconds();
        var batchDateTime = now.ToString("yyyy-MM-dd hh:mm:ss.fff tt");

        var callOrderBookOptionBuying = _context.OrderBookOptionBuying.ToList().Where(a => a.closeBatch <= 0 && DateTime.Parse(a.optionDate, new CultureInfo("en-US", true)) == DateTime.Parse(optionDate, new CultureInfo("en-US", true))).ToList();
        foreach (var order in callOrderBookOptionBuying)
        {
            var option = this.GetOptionDetails(_context, users.First(), order.optionDate, order.symbol, order.strikePrice, order.strikePrice, order.optionType, batchId, batchDateTime);
            this.CloseOrderOptionBuying(_context, order, option, order.id);
        }

        var callOrderBookOptionSelling = _context.OrderBookOptionSelling.ToList().Where(a => a.closeBatch <= 0 && DateTime.Parse(a.optionDate, new CultureInfo("en-US", true)) == DateTime.Parse(optionDate, new CultureInfo("en-US", true))).ToList();
        foreach (var order in callOrderBookOptionSelling)
        {
            var option = this.GetOptionDetails(_context, users.First(), order.optionDate, order.symbol, order.strikePrice, order.strikePrice, order.optionType, batchId, batchDateTime);
            this.CloseOrderOptionSelling(_context, order, option, order.id);
        }        
    }

    public void PercentageCheck(CoreDbContext _context, string optionDate)
    {
        var users = _context.User.ToList();
        DateTimeOffset now = (DateTimeOffset)Help.GetEstDatetime();
        var batchId = now.ToUnixTimeSeconds();
        var batchDateTime = now.ToString("yyyy-MM-dd hh:mm:ss.fff tt");      
            
        var callOrderBookOptionBuying = _context.OrderBookOptionBuying.ToList().Where(a => a.closeBatch <= 0 && DateTime.Parse(a.optionDate, new CultureInfo("en-US", true)) == DateTime.Parse(optionDate, new CultureInfo("en-US", true))).ToList();
        foreach (var order in callOrderBookOptionBuying)
        {
            if(order.actualPnL5 <= 0)
            {
                var option = this.GetOptionDetails(_context, users.First(), order.optionDate, order.symbol, order.strikePrice, order.strikePrice, order.optionType, batchId, batchDateTime, false);
                OrderBookOptionBuyingUpdatePercentage(_context, batchId, order, option);
            }
           
        }

        var callOrderBookOptionSelling = _context.OrderBookOptionSelling.ToList().Where(a => a.closeBatch <= 0 && DateTime.Parse(a.optionDate, new CultureInfo("en-US", true)) == DateTime.Parse(optionDate, new CultureInfo("en-US", true))).ToList();
        foreach (var order in callOrderBookOptionSelling)
        {
            if (order.actualPnL5 <= 0)
            {
                var option = this.GetOptionDetails(_context, users.First(), order.optionDate, order.symbol, order.strikePrice, order.strikePrice, order.optionType, batchId, batchDateTime, false);
                OrderBookOptionSellingUpdatePercentage(_context, batchId, order, option);
            }
        }
    }

    private static void OrderBookOptionBuyingUpdatePercentage(CoreDbContext _context, long batchId, OrderBookOptionBuying? order, Option option)
    {
        if (order != null)
        {
            bool changeFlag = false;
            if (order.batch1 <= 0 && (option.Bid - order.openCost) > order.expectedPnL1)
            {
                order.actualPnL1 = option.Bid - order.openCost;
                order.batch1 = batchId;
                changeFlag = true;
            }
            if (order.batch2 <= 0 && (option.Bid - order.openCost) > order.expectedPnL2)
            {
                order.actualPnL2 = option.Bid - order.openCost;
                order.batch2 = batchId;
                changeFlag = true;
            }
            if (order.batch3 <= 0 && (option.Bid - order.openCost) > order.expectedPnL3)
            {
                order.actualPnL3 = option.Bid - order.openCost;
                order.batch3 = batchId;
                changeFlag = true;
            }
            if (order.batch4 <= 0 && (option.Bid - order.openCost) > order.expectedPnL4)
            {
                order.actualPnL4 = option.Bid - order.openCost;
                order.batch4 = batchId;
                changeFlag = true;
            }
            if (order.batch5 <= 0 && (option.Bid - order.openCost) > order.expectedPnL5)
            {
                order.actualPnL5 = option.Bid - order.openCost;
                order.batch5 = batchId;
                changeFlag = true;
            }

            if (changeFlag)
            {
                _context.Option.Add(option);
                _context.SaveChanges();

                _context.Update(order);
                _context.SaveChanges();
            }
        }
    }

    private static void OrderBookOptionSellingUpdatePercentage(CoreDbContext _context, long batchId, OrderBookOptionSelling? order, Option option)
    {
        if (order != null)
        {
            bool changeFlag = false;
            if (order.batch1 <= 0 && (option.Ask - order.openCost) > order.expectedPnL1)
            {
                order.actualPnL1 = option.Ask - order.openCost;
                order.batch1 = batchId;
                changeFlag = true;
            }
            if (order.batch2 <= 0 && (option.Ask - order.openCost) > order.expectedPnL2)
            {
                order.actualPnL2 = option.Ask - order.openCost;
                order.batch2 = batchId;
                changeFlag = true;
            }
            if (order.batch3 <= 0 && (option.Ask - order.openCost) > order.expectedPnL3)
            {
                order.actualPnL3 = option.Ask - order.openCost;
                order.batch3 = batchId;
                changeFlag = true;
            }
            if (order.batch4 <= 0 && (option.Ask - order.openCost) > order.expectedPnL4)
            {
                order.actualPnL4 = option.Ask - order.openCost;
                order.batch4 = batchId;
                changeFlag = true;
            }
            if (order.batch5 <= 0 && (option.Ask - order.openCost) > order.expectedPnL5)
            {
                order.actualPnL5 = option.Ask - order.openCost;
                order.batch5 = batchId;
                changeFlag = true;
            }

            if (changeFlag)
            {
                _context.Option.Add(option);
                _context.SaveChanges();

                _context.Update(order);
                _context.SaveChanges();

                
            }
        }
    }
}

