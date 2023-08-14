using Azure.Core;
using DotNetCoreSqlDb.Data;
using DotNetCoreSqlDb.Models;
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

    public Option GetOptionDetails(CoreDbContext context, User user, string optionDate, string symbol, double price, double strikePriceNear, string chainType, long batchId, string batchDateTime)
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

            var quoteRequest = new RestRequest($"v1/market/optionchains?symbol={symbol}&expiryYear={expiryYear}&expiryMonth={expiryMonth}&expiryDay={expiryDay}&includeWeekly={false}&chainType={chainType}&strikePriceNear={strikePriceNear}&noOfStrikes={noOfStrikes}");
            client.Authenticator = OAuth1Authenticator.ForProtectedResource(user.ConsumerKey, user.ConsumerSecret, user.AccessToken, user.AccessTokenSecret);
            var response = client.Execute(quoteRequest);

            optionJson = JObject.Parse(response.Content);
            options = BuildOptionBase(batchId, batchDateTime, price, optionJson.ToString());
           
            context.Option.AddRange(options);
            context.SaveChanges();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return options.First();    }


    public void OpenOrder(CoreDbContext context, Option option, int singalId)
    {

        var orderBookDemo = new OrderBookDemo
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
            openDateTime = DateTime.Now,
            closeBatchDateTime = option.BatchDateTime
        };

        context.Add(orderBookDemo);
        context.SaveChanges();
    }

    public void CloseOrder(CoreDbContext context, OrderBookDemo orderBookDemo, Option option, int singalId)
    {
        orderBookDemo.closeBatch = option.Batch;
        orderBookDemo.closeBatchDateTime = option.BatchDateTime;
        orderBookDemo.closeDateTime = DateTime.Now;
        orderBookDemo.closeSinalId = singalId;
        orderBookDemo.closeStokePrice = option.Price;
        orderBookDemo.closeCost = option.Bid;
        orderBookDemo.PnL = orderBookDemo.closeCost - orderBookDemo.openCost;       

        context.Update(orderBookDemo);
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

    public string HealthCheck(User user, string symbol, DateTimeOffset now)
    {
        List<Option> options = new List<Option>();
        JObject optionJson = new JObject();
        try
        {            
            string etradeBaseUrl = user.EtradeBaseUrl;
            var client = new RestClient(etradeBaseUrl);

            var quoteRequest = new RestRequest($"v1/market/quote/{symbol}");
            client.Authenticator = OAuth1Authenticator.ForProtectedResource(user.ConsumerKey, user.ConsumerSecret, user.AccessToken, user.AccessTokenSecret);
            var response = client.Execute(quoteRequest);

            if (response.Content.Contains("oauth_problem"))
                Console.WriteLine($"=========oauth_problem: {DateTimeOffset.Now}");


            var stockJson = JObject.Parse(response.Content).First.First;
            var price = Convert.ToDouble(stockJson["QuoteData"][0]["All"]["ask"]);
            price = price <= 0 ? Convert.ToDouble(stockJson["QuoteData"][0]["All"]["lastTrade"]) : price;

            return price.ToString() + " : " + now.ToString();


        }
        catch (Exception ex)
        {
            return "Error:" + ex.Message;            
        }
    }
}

