namespace DotNetCoreSqlDb.Models
{
    public class OrderBookOptionSelling
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
        public string PnLPercentage { get; set; }

        public string percentFlag1 { get; set; }
        public double expectedPnL1 { get; set; }
        public double actualPnL1 { get; set; }
        public long batch1 { get; set; }

        public string percentFlag2 { get; set; }
        public double expectedPnL2 { get; set; }
        public double actualPnL2 { get; set; }
        public long batch2 { get; set; }

        public string percentFlag3 { get; set; }
        public double expectedPnL3 { get; set; }
        public double actualPnL3 { get; set; }
        public long batch3 { get; set; }

        public string percentFlag4 { get; set; }
        public double expectedPnL4 { get; set; }
        public double actualPnL4 { get; set; }
        public long batch4 { get; set; }

        public string percentFlag5 { get; set; }
        public double expectedPnL5 { get; set; }
        public double actualPnL5 { get; set; }
        public long batch5 { get; set; }
    }
}
