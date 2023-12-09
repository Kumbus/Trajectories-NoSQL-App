namespace Backend.Model
{
    public class FullSimpleQueryResult
    {
        public string Name { get; set; }
        public decimal FullTime { get; set; }
        public decimal QueryTime { get; set; }
        public int Count { get; set; }
        public int MaxPage { get; set; }
        public List<SimpleQueryResult> Results { get; set; }
    }
}
