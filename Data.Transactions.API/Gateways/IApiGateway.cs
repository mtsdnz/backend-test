using System.Threading.Tasks;
using Core.ActionsResult;

namespace Data.Transactions.API.Gateways
{
    public interface IApiGateway
    {
        Task<Result<string>> GetClientPayments();

        Task<Result<string>> GetClientReceivedPayments();
    }
}