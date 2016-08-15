namespace Quandl.Shared.Models
{
    public class DataColumn
    {
        string Name { get; set; }

        // This is not working for datatable metadata call
        //ProviderType Type { get; set; }

       string Type { get; set; }
    }
}