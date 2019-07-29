using System.Collections.Generic;
using System.Threading.Tasks;
using Core.ActionsResult;
using Core.Models;

namespace Core.Services
{
    public interface ITransactionService
    {
        Task<Result<List<TransactionData>>> GetTransactions();
    }
}