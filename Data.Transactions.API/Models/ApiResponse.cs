using Newtonsoft.Json;

namespace Data.Transactions.API.Models
{
    public class ApiResponse
    {
        [JsonProperty("data")] public string Date { get; set; }

        [JsonProperty("moeda")] public string Currency { get; set; }

        [JsonProperty("modeda")] public string CurrencyWrapper { get; set; }

        [JsonProperty("valor")] public string Amount { get; set; }

        [JsonProperty("descricao")] public string Description { get; set; }

        [JsonProperty("categoria")] public string Category { get; set; }
    }
}