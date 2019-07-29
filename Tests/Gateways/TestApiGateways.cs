using System.Collections.Generic;
using System.Threading.Tasks;
using Core.ActionsResult;
using Data.Transactions.API.Gateways;
using Data.Transactions.API.Models;
using Newtonsoft.Json;

namespace Tests.Gateways
{
    public class TestApiGateways : IApiGateway
    {
        public string PaymentResponse = "";

        public string ReceivedPaymentsResponse = "";

        public void SetSamplePaymentResponse(List<ApiResponse> response)
        {
            PaymentResponse = JsonConvert.SerializeObject(response);
        }

        public void SetSamplePaymentReceivedResponse(List<ApiResponse> response)
        {
            ReceivedPaymentsResponse = JsonConvert.SerializeObject(response);
        }

        public async Task<Result<string>> GetClientPayments()
        {
            return HandleResponse(PaymentResponse, "Invalid payment response");
        }

        public async Task<Result<string>> GetClientReceivedPayments()
        {
            return HandleResponse(ReceivedPaymentsResponse, "Invalid Received payment response");
        }

        private Result<string> HandleResponse(string response, string error)
        {
            return string.IsNullOrEmpty(response) ? Result<string>.Fail(null, error) : Result<string>.Ok(response);
        }
    }
}