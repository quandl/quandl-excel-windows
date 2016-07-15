using System.Collections.Generic;

namespace Quandl.Shared.Models.Browse
{
    public class NamedContentCollection
    {
        public List<NamedContent> NamedContents { get; set; }
    }

    public class NamedContent
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string HtmlContent { get; set; }
    }
}
