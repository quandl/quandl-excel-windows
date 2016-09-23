using System.Collections.Generic;

namespace Quandl.Test.CodedUI.Helpers
{
    public partial class CodedUITestHelpers
    {
        public static string convertListToUDFArray(List<string> content)
        {
            return $"{{\"{string.Join("\",\"", content)}\"}}";
        }
    }
}
