namespace Quandl.Shared.models
{
    public class Meta
    {
        public int PerPage { get; set; }
        public string Query { get; set; }
        public int CurrentPage { get; set; }
        public object PrevPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public int NextPage { get; set; }
        public int CurrentFirstItem { get; set; }
        public int CurrentLastItem { get; set; }
    }
}
