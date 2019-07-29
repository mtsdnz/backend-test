using System.Threading.Tasks;
using Core.ActionsResult;
using Data.Transactions.API.Helpers;
using Data.Transactions.API.Models;

namespace Data.Transactions.API.Gateways
{
    public class ApiGateway : IApiGateway
    {
        private readonly ApiConfig _config;

        public ApiGateway(ApiConfig config)
        {
            _config = config;
        }

        /// <summary>
        /// Obtem os pagamentos realizados pelo client.
        /// </summary>
        public async Task<Result<string>> GetClientPayments()
        {
            return await HttpHelper.DownloadStringAsync(_config.ClientPaymentsUrl);
        }

        /// <summary>
        /// Obtem os pagamentos recebidos pelo client.
        /// </summary>
        public async Task<Result<string>> GetClientReceivedPayments()
        {
            return await HttpHelper.DownloadStringAsync(_config.ClientReceivedPaymentsUrl);
        }
    }
}