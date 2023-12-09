namespace Backend.Model
{
    public class FullAggregateQueryResult
    {
        public string Name { get; set; }
        public decimal FullTime { get; set; }
        public decimal QueryTime { get; set; }
        public int Count { get; set; }
        public List<AggregateQueryResult> Results { get; set; }
    }
}
