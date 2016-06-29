namespace Quandl.Shared.models
{
    public class Database
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DatabaseCode { get; set; }
        public string Description { get; set; }
        public long DatasetsCount { get; set; }
        public long Downloads { get; set; }
        public bool Premium { get; set; }
        public string Image { get; set; }
    }
}
