using System.ComponentModel.DataAnnotations.Schema;

namespace DotNetCoreSqlDb.Models
{
    public class EMASignal
    {
        public int id { get; set; }
        public string Simbol { get; set; }
        public DateTime OptionDate { get; set; }
        public int Length { get; set; }
        public string Signal { get; set; }
        public DateTime SignalDatetime { get; set; }

        [NotMapped]
        public DateTime CreatedAt { get; set; }
    }
}
