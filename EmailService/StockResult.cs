using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailService
{
    public class StockResult
    {
        public int Id { get; set; } // Primary key
        public string T { get; set; } // Stock symbol
        public int V { get; set; } // Volume
        public decimal Vw { get; set; } // Volume-weighted average price
        public decimal O { get; set; } // Opening price
        public decimal C { get; set; } // Closing price
        public decimal H { get; set; } // Highest price
        public decimal L { get; set; } // Lowest price
        public DateTime Timestamp { get; set; } // Converted from Unix timestamp
        public int N { get; set; } // Number of transactions

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Default to current UTC time
    }
}
