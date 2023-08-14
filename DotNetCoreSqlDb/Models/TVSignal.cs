﻿using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace DotNetCoreSqlDb.Models
{
    public class TVSignal
    {
        public int id { get; set; }
        public string Simbol { get; set; }
        public double Price { get; set; }
        public double Volume { get; set; }
        public string Period { get; set; }
        public string Signal { get; set; }
        public string Source { get; set; }
        public DateTime SignalDatetime { get; set; }

        [NotMapped]
        public DateTime CreatedAt { get; set; }        
    }
}
