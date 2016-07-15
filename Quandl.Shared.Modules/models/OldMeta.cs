namespace Quandl.Shared.Models
{
    public class OldMeta
    {
        public int? PerPage { get; set; }
        public string Query { get; set; }
        public int? CurrentPage { get; set; }
        public int? PrevPage { get; set; }
        public int? TotalPages { get; set; }
        public int? NextPage { get; set; }
        public int CurrentFirstItem { get; set; }
        public int CurrentLastItem { get; set; }
    }
}
