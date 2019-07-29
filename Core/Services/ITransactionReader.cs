using System.Collections.Generic;
using System.Threading.Tasks;
using Core.ActionsResult;
using Core.Models;

namespace Core.Services
{
    public interface ITransactionReader
    {
        Task<Result<List<TransactionData>>> GetTransactions();
    }
}