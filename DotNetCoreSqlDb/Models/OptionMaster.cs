namespace DotNetCoreSqlDb.Models;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public partial class OptionChain
{
    [JsonProperty("OptionChainResponse")]
    public OptionChainResponse OptionChainResponse { get; set; }
}

public partial class OptionChainResponse
{
    [JsonProperty("timeStamp")]
    public long TimeStamp { get; set; }

    [JsonProperty("quoteType")]
    public string QuoteType { get; set; }

    [JsonProperty("nearPrice")]
    public long NearPrice { get; set; }

    [JsonProperty("OptionPair")]
    public OptionPair[] OptionPair { get; set; }

    [JsonProperty("SelectedED")]
    public SelectedEd SelectedEd { get; set; }
}

public partial class SelectedEd
{
    [JsonProperty("month")]
    public long Month { get; set; }

    [JsonProperty("year")]
    public long Year { get; set; }

    [JsonProperty("day")]
    public long Day { get; set; }
}

public partial class OptionPair
{
    [JsonProperty("Call")]
    public Option Call { get; set; }

    [JsonProperty("Put")]
    public Option Put { get; set; }

    [JsonProperty("batch")]
    public long Batch { get; set; }

    [JsonProperty("price")]
    public double Price { get; set; }
}

[Table("OptionBase", Schema = "dbo")]
public class OptionBase
{
    [Key]
    public int Id { get; set; }
    [JsonProperty("optionCategory")]

    public string OptionCategory { get; set; }

    [JsonProperty("optionRootSymbol")]
    public string OptionRootSymbol { get; set; }

    [JsonProperty("batch")]
    public long Batch { get; set; }

    [JsonProperty("batchDateTime")]
    public string BatchDateTime { get; set; }

    [JsonProperty("price")]
    public double Price { get; set; }
    
    //public bool PriceRoundFlag { get; set; }


    [JsonProperty("timeStamp")]
    public long TimeStamp { get; set; }

    [JsonProperty("adjustedFlag")]
    public bool AdjustedFlag { get; set; }

    [JsonProperty("displaySymbol")]
    public string DisplaySymbol { get; set; }

    [JsonProperty("optionType")]
    public string OptionType { get; set; }

    [JsonProperty("strikePrice")]
    public long StrikePrice { get; set; }

    [JsonProperty("symbol")]
    public string Symbol { get; set; }

    [JsonProperty("bid")]
    public double Bid { get; set; }

    [JsonProperty("ask")]
    public double Ask { get; set; }

    [JsonProperty("bidSize")]
    public long BidSize { get; set; }

    [JsonProperty("askSize")]
    public long AskSize { get; set; }

    [JsonProperty("inTheMoney")]
    public string InTheMoney { get; set; }

    [JsonProperty("volume")]
    public long Volume { get; set; }

    [JsonProperty("openInterest")]
    public long OpenInterest { get; set; }

    [JsonProperty("netChange")]
    public double NetChange { get; set; }

    [JsonProperty("lastPrice")]
    public double LastPrice { get; set; }

    [JsonProperty("quoteDetail")]
    public Uri QuoteDetail { get; set; }

    [JsonProperty("osiKey")]
    public string OsiKey { get; set; }

    [JsonProperty("rho")]
    public double Rho { get; set; }

    [JsonProperty("vega")]
    public double Vega { get; set; }

    [JsonProperty("theta")]
    public double Theta { get; set; }

    [JsonProperty("delta")]
    public double Delta { get; set; }

    [JsonProperty("gamma")]
    public double Gamma { get; set; }

    [JsonProperty("iv")]
    public double Iv { get; set; }

    [JsonProperty("currentValue")]
    public bool CurrentValue { get; set; }

    [JsonProperty("minutesToExpire")]
    public int MinutesToExpire { get; set; } 

    [JsonProperty("optionDate")]
    public string OptionDate { get; set; }

    [JsonProperty("optionPrice")]
    public double OptionPrice { get; set; } 
}

[Table("OptionBaseSpy", Schema = "dbo")]
public class OptionBaseSpy
{
    [Key]
    public int Id { get; set; }
    [JsonProperty("optionCategory")]


    public string OptionCategory { get; set; }

    [JsonProperty("optionRootSymbol")]
    public string OptionRootSymbol { get; set; }

    [JsonProperty("batch")]
    public long Batch { get; set; }

    [JsonProperty("batchDateTime")]
    public string BatchDateTime { get; set; }

    [JsonProperty("price")]
    public double Price { get; set; }

    //public bool PriceRoundFlag { get; set; }


    [JsonProperty("timeStamp")]
    public long TimeStamp { get; set; }

    [JsonProperty("adjustedFlag")]
    public bool AdjustedFlag { get; set; }

    [JsonProperty("displaySymbol")]
    public string DisplaySymbol { get; set; }

    [JsonProperty("optionType")]
    public string OptionType { get; set; }

    [JsonProperty("strikePrice")]
    public long StrikePrice { get; set; }

    [JsonProperty("symbol")]
    public string Symbol { get; set; }

    [JsonProperty("bid")]
    public double Bid { get; set; }

    [JsonProperty("ask")]
    public double Ask { get; set; }

    [JsonProperty("bidSize")]
    public long BidSize { get; set; }

    [JsonProperty("askSize")]
    public long AskSize { get; set; }

    [JsonProperty("inTheMoney")]
    public string InTheMoney { get; set; }

    [JsonProperty("volume")]
    public long Volume { get; set; }

    [JsonProperty("openInterest")]
    public long OpenInterest { get; set; }

    [JsonProperty("netChange")]
    public double NetChange { get; set; }

    [JsonProperty("lastPrice")]
    public double LastPrice { get; set; }

    [JsonProperty("quoteDetail")]
    public Uri QuoteDetail { get; set; }

    [JsonProperty("osiKey")]
    public string OsiKey { get; set; }

    [JsonProperty("rho")]
    public double Rho { get; set; }

    [JsonProperty("vega")]
    public double Vega { get; set; }

    [JsonProperty("theta")]
    public double Theta { get; set; }

    [JsonProperty("delta")]
    public double Delta { get; set; }

    [JsonProperty("gamma")]
    public double Gamma { get; set; }

    [JsonProperty("iv")]
    public double Iv { get; set; }

    [JsonProperty("currentValue")]
    public bool CurrentValue { get; set; }

    [JsonProperty("minutesToExpire")]
    public int MinutesToExpire { get; set; }

    [JsonProperty("optionDate")]
    public string OptionDate { get; set; }

    [JsonProperty("optionPrice")]
    public double OptionPrice { get; set; }
}

public partial class Option : OptionBase
{
    [JsonProperty("OptionGreeks")]
    public OptionGreeks OptionGreeks { get; set; }

    [JsonProperty("rho")]
    public double Rho { get { return this.OptionGreeks.Rho; } set { base.Rho = value; } }

    [JsonProperty("vega")]
    public double Vega { get { return this.OptionGreeks.Vega; } set { base.Vega = value; } }

    [JsonProperty("theta")]
    public double Theta { get { return this.OptionGreeks.Theta; } set { base.Theta = value; } }

    [JsonProperty("delta")]
    public double Delta { get { return this.OptionGreeks.Delta; } set { base.Delta = value; } }

    [JsonProperty("gamma")]
    public double Gamma { get { return this.OptionGreeks.Gamma; } set { base.Gamma = value; } }

    [JsonProperty("iv")]
    public double Iv { get { return this.OptionGreeks.Iv; } set { base.Iv = value; } }

    [JsonProperty("currentValue")]
    public bool CurrentValue { get { return this.OptionGreeks.CurrentValue; } set { base.CurrentValue = value; } }
        
    public string[] QuoteDetailArrary { get { return this.QuoteDetail.ToString().Split(":"); }  }

    [JsonProperty("minutesToExpire")]
    public int MinutesToExpire { get; set; } = 100;

    [JsonProperty("optionDate")]
    public string OptionDate { get { return new DateTime(Convert.ToInt32(QuoteDetailArrary[2]), Convert.ToInt32(QuoteDetailArrary[3]), Convert.ToInt32(QuoteDetailArrary[4])).ToString("yyyy-MM-dd"); ; } set { base.OptionDate = value; } }
   
    [JsonProperty("optionPrice")]
    public double OptionPrice { get { return Convert.ToDouble(QuoteDetailArrary[6]); } set { base.OptionPrice = value; } }

}

public partial class OptionGreeks
{
    double _Rho;
    [JsonProperty("rho")]
    public double Rho {
        get
        {
            if (Math.Round(this._Rho, 0) == -9999999)
            {
                return 0.00001;
            }
            else
            {
                return this._Rho; ;
            }

        }
        set
        {
            _Rho = value;
        }
    }

    double _Vega;
    [JsonProperty("vega")]
    public double Vega {
        get
        {
            if (Math.Round(this._Vega, 0) == -9999999)
            {
                return 0.00001;
            }
            else
            {
                return this._Vega; ;
            }

        }
        set
        {
            _Vega = value;
        }
    }

    double _Theta;
    [JsonProperty("theta")]
    public double Theta {
        get
        {
            if (Math.Round(this._Theta, 0) == -9999999)
            {
                return 0.00001;
            }
            else
            {
                return this._Theta; ;
            }

        }
        set
        {
            _Theta = value;
        }
    }

    double _Delta;
    [JsonProperty("delta")]     
    public double Delta
    {
        get
        {
            if (Math.Round(this._Delta, 0) == -9999999)
            {
                return 0.00001;
            }
            else
            {
                return this._Delta;
            }

        }
        set
        {
            _Delta = value;
        }
    }

    double _Gamma;
    [JsonProperty("gamma")]
    public double Gamma
    {
        get
        {
            if (Math.Round(this._Gamma, 0) == -9999999)
            {
                return 0.00001;
            }
            else
            {
                return this._Gamma;
            }
        }
        set
        {
            _Gamma = value;
        }
    }

    double _Iv;
    [JsonProperty("iv")]
    public double Iv {
        get
        {
            if (Math.Round(this._Iv, 0) == -9999999)
            {
                return 0.00001;
            }
            else
            {
                return this._Iv;
            }
        }
        set
        {
            _Iv = value;
        }
    }

    [JsonProperty("currentValue")]
    public bool CurrentValue { get; set; }
}

[Table("OptionStrangleAlert", Schema = "dbo")]
public class OptionStrangleAlert
{
    [Key]
    public int Id { get; set; }

    [JsonProperty("batch")]
    public long Batch { get; set; }

    [JsonProperty("batchDateTime")]
    public string BatchDateTime { get; set; }

    [JsonProperty("symbol")]
    public string Symbol { get; set; }

    [JsonProperty("price")]
    public double Price { get; set; }

    [JsonProperty("optionDate")]
    public string OptionDate { get; set; }

    [JsonProperty("length")]
    public double Length { get; set; }    

    [JsonProperty("strikePrice")]
    public string StrikePrice { get; set; }

    [JsonProperty("spreadPrice")]
    public double SpreadPrice { get; set; }

    [JsonProperty("percent1Up")]
    public double Percent1Up { get; set; }

    [JsonProperty("percent1Down")]
    public double Percent1Down { get; set; }

    [JsonProperty("percent2Up")]
    public double Percent2Up { get; set; }

    [JsonProperty("percent2Down")]
    public double Percent2Down { get; set; }

    [JsonProperty("notes")]
    public string Notes { get; set; }
}

[Table("OptionStrangle", Schema = "dbo")]
public class OptionStrangle
{
    [Key]
    public int Id { get; set; }

    [JsonProperty("batch")]
    public long Batch { get; set; }

    [JsonProperty("batchDateTime")]
    public string BatchDateTime { get; set; }

    [JsonProperty("symbol")]
    public string Symbol { get; set; }

    [JsonProperty("price")]
    public double Price { get; set; }

    [JsonProperty("optionDate")]
    public string OptionDate { get; set; }

    [JsonProperty("length")]
    public double Length { get; set; }

    [JsonProperty("strikePrice")]
    public string StrikePrice { get; set; }

    [JsonProperty("putDistance")]
    public double putDistance { get; set; }

    [JsonProperty("midDistance")]
    public double midDistance { get; set; }

    [JsonProperty("callDistance")]
    public double callDistance { get; set; }

    [JsonProperty("hourToExpiration")]
    public double hourToExpiration { get; set; }

    [JsonProperty("bid")]
    public string Bid { get; set; }

    [JsonProperty("ask")]
    public string Ask { get; set; }

    [JsonProperty("netBid")]
    public double NetBid { get; set; }

    [JsonProperty("netAsk")]
    public double NetAsk { get; set; }

    [JsonProperty("priceChange")]
    public double priceChange { get; set; }

    [JsonProperty("expectedProfit")]
    public double ExpectedProfit { get; set; }

    [JsonProperty("expectedPercent")]
    public double ExpectedPercent { get; set; }

    [JsonProperty("openInterest")]
    public string OpenInterest { get; set; }

    [JsonProperty("iv")]
    public string Iv { get; set; }

    [JsonProperty("delta")]
    public string Delta { get; set; }

    [JsonProperty("theta")]
    public string Theta { get; set; }

    [JsonProperty("vega")]
    public string Vega { get; set; }

    [JsonProperty("gamma")]
    public string Gamma { get; set; }

    [JsonProperty("netIv")]
    public double NetIv { get; set; }

    [JsonProperty("netDelta")]
    public double NetDelta { get; set; }

    [JsonProperty("netTheta")]
    public double NetTheta { get; set; }

    [JsonProperty("netVega")]
    public double NetVega { get; set; }

    [JsonProperty("netGamma")]
    public double NetGamma { get; set; }
}

public class OptionCalender
{
    [Key]
    public int Id { get; set; }

    private DateTime _optionDate;
    [JsonProperty("optionDate")]
    public DateTime OptionDate { get { return _optionDate; } set { _optionDate = value; } }

    public string OptionDateString()
    {
        return this._optionDate.ToString("yyyy-MM-dd");
    }

    [JsonProperty("weekly")]
    public bool Weekly { get; set; }

    [JsonProperty("monthly")]
    public bool Monthly { get; set; }

    //[JsonProperty("hourToExpiration")]
    public decimal hourToExpiration { get; set; }

}

class WorkerSetting
{
    public DateTime OptionDate { get; set; }
    public List<int> Length { get; set; }
    public int Delay { get; set; }

    public int NumberOfStrikes { get; set; }
}

