namespace Data.Transactions.API.Models
{
    public struct ApiConfig
    {
        public ApiConfig(string clientPaymentsUrl, string clientReceivedPaymentsUrl)
        {
            ClientPaymentsUrl = clientPaymentsUrl;
            ClientReceivedPaymentsUrl = clientReceivedPaymentsUrl;
        }

        public string ClientPaymentsUrl { get; }

        public string ClientReceivedPaymentsUrl { get; }
    }
}