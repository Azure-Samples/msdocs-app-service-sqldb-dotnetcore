using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DotNetCoreSqlDb.Models
{
    public class ElectricalTestResult
    {
        public int Id { get; set; } // Primary Key for database
        public string JobNameOrNumber { get; set; }
        public string? CircuitLocation { get; set; }
        public string? CircuitNameOrDesignation { get; set; }
        public string? VisualInspection { get; set; }
        public string? ProtectionSizeOrType { get; set; }
        public string? NeutralNumber { get; set; }
        public int? NumberOfPhases { get; set; }
        public string? CableSize { get; set; }
        public string? EarthSize { get; set; }

        // Changed to nullable boolean
        public bool? ShortCircuitPass { get; set; }
        public bool? InterconnectPass { get; set; }
        public bool? PolarityPass { get; set; }

        public double? ContinuityOhms { get; set; }
        public double? InsulationResistance { get; set; }
        public double? FaultLoopImpedance { get; set; }
        public double? RcdTripTime { get; set; }

        [DisplayName("Created Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime CreatedDate { get; set; }
        public string UserId { get; set; } // Foreign Key to User table

        // TimeStamp property automatically set when a new instance is created
        public DateTime TimeStamp { get; private set; } = DateTime.UtcNow;
    }
}

