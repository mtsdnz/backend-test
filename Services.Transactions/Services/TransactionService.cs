using System.Collections.Generic;
using System.Threading.Tasks;
using Core.ActionsResult;
using Core.Models;
using Core.Services;

namespace Services.Transactions
{
    public class TransactionService : ITransactionService
    {
        private readonly IEnumerable<ITransactionReader> _readers;

        public TransactionService(IEnumerable<ITransactionReader> readers)
        {
            _readers = readers;
        }

        public async Task<Result<List<TransactionData>>> GetTransactions()
        {
            var transactions = new List<TransactionData>();

            foreach (var transactionReader in _readers)
            {
                var result = await transactionReader.GetTransactions();
                //se não consegui ler, ele retorna o resultado.
                if (!result.IsSuccess)
                    return result;

                if (result.Value != null)
                    transactions.AddRange(result.Value);
            }

            return Result<List<TransactionData>>.Ok(transactions);
        }
    }
}