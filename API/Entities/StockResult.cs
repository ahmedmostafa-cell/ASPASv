namespace API.Entities
{
    public class StockResult
    {
        public int Id { get; set; }
        public string T { get; set; }
        public int V { get; set; }
        public decimal Vw { get; set; }
        public decimal O { get; set; }
        public decimal C { get; set; }
        public decimal H { get; set; }
        public decimal L { get; set; }
        public DateTime Timestamp { get; set; }
        public int N { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
