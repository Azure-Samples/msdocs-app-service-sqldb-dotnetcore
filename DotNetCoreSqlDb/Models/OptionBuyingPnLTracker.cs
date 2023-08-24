namespace DotNetCoreSqlDb.Models
{
    public class OptionBuyingPnLTracker
    {
        public int id { get; set; }
        public int parentId { get; set; }
        public string dateTime { get; set; }
        public double PnL { get; set; }
        public string PnLPercentage { get; set; }
        public long batch { get; set; }
    }
}
