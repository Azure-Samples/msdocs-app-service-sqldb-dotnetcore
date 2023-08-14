namespace DotNetCoreSqlDb.Models
{
    public class OrderBookDemo
    {
        public int id { get; set; }
        public string symbol { get; set; }
        public string optionType { get; set; }
        public string optionDate { get; set; }
        public double strikePrice { get; set; }
        public DateTime openDateTime { get; set; }
        public long openBatch { get; set; }
        public string openBatchDateTime { get; set; }
        public int openSinalId { get; set; }
        public double openStokePrice { get; set; }
        public double openCost { get; set; }
        public DateTime closeDateTime { get; set; }
        public long closeBatch { get; set; }
        public string closeBatchDateTime { get; set; }
        public int closeSinalId { get; set; }
        public double closeStokePrice { get; set; }
        public double closeCost { get; set; }
        public double PnL { get; set; }
    }
}
