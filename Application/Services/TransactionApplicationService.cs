using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.ActionsResult;
using Core.Extensions;
using Core.Models;
using Core.Services;

namespace Application.Services
{
    public class TransactionApplicationService
    {
        private readonly ITransactionService _transactionService;

        private readonly ILoggerService _logger;

        private List<TransactionData> _cachedTransactions;

        public TransactionApplicationService(ITransactionService transactionService, ILoggerService logger = null)
        {
            _transactionService = transactionService;
            _logger = logger;
        }

        public async Task<List<TransactionData>> GetTransactions()
        {
            if (_cachedTransactions != null)
                return _cachedTransactions;

            var result = await _transactionService.GetTransactions();
            if (!result.IsSuccess)
            {
                _logger?.WriteError(string.Join(Environment.NewLine, result.Errors));
                return new List<TransactionData>();
            }

            return _cachedTransactions = result.Value;
        }

        private async Task<List<TransactionData>> GetPayments()
        {
            return (await GetTransactions()).Where(e => e.Amount < 0).ToList();
        }

        private async Task<List<TransactionData>> GetReceivedPayments()
        {
            return (await GetTransactions()).Where(e => e.Amount > 0).ToList();
        }

        /// <summary>
        /// Obtem o total de gastos por categoria. 
        /// </summary>
        /// <returns>Dicionario, sendo, Key = "Nome da Categoria" e Value = "Gastos da categoria"</returns>
        public async Task<Dictionary<string, double>> GetSpentValuesByCategoryOrdered()
        {
            return (await GetPayments()).GroupBy(e =>
                    e.Category
                        .RemoveDiacritics()) //Agrupa por categoria, e remove os acentos, para evitar que 'alimentacao' e 'alimentaçao' sejam lidos de formas diferentes.
                .Select(category => new
                {
                    Category = string.IsNullOrWhiteSpace(category.Key)
                        ? "Outros" // Se o nome da categoria estiver vazio, deixamos como "Outros"
                        : category.Key, //Nome da categoria
                    Amount = category.Sum(e => Math.Abs(e.Amount)) // Soma os gastos da categoria
                })
                .OrderByDescending(e => e.Amount) //Ordena os valores, do maior para o menor.
                .ToDictionary(k => k.Category, v => v.Amount);
        }

        /// <summary>
        /// Obtem a categoria que o cliente gastou mais
        /// </summary>
        /// <returns>Retorna <see cref="KeyValuePair{TKey,TValue}"/> onde Key = Nome da categoria, e Value = valor gasto pela categoria </returns>
        public async Task<KeyValuePair<string, double>> GetMostSpentCategory()
        {
            return (await GetSpentValuesByCategoryOrdered()).FirstOrDefault();
        }

        /// <summary>
        /// Obtem as transações do cliente, de forma ordenada.
        /// </summary>
        public async Task<List<TransactionData>> GetTransactionsOrdered()
        {
            return (await GetTransactions()).OrderByDescending(e => e.Date).ToList();
        }

        /// <summary>
        /// Obtem o mês e o valor, em que o cliente mais gastou.
        /// </summary>
        /// <returns><see cref="KeyValuePair{TKey,TValue}"/> onde Key = Nome do mês, e Value = valor gasto no mês.
        /// Caso o cliente não tenha nenhuma transação, retornará o mês atual, e o valor = 0;
        /// </returns>
        public async Task<KeyValuePair<string, double>> GetMostSpentMonth()
        {
            var mostSpentMonth = (await GetPayments())
                //Agrupa os por mês
                .GroupBy(t => t.Date.ToString("MMMM", CultureInfo.CurrentCulture))
                //Cria um objeto anonimo, com nome do mês, e o total gasto
                .Select(transaction => new
                {
                    Month = transaction.Key,
                    Amount = transaction.Sum(tm => Math.Abs(tm.Amount))
                })
                //Ordena pelo total gasto, do maior para o menor.
                .OrderByDescending(e => e.Amount)
                //Obtem o primeiro documento
                .FirstOrDefault();

            //Se o mês for nulo, retorna o mês atual, e o valor = 0
            return new KeyValuePair<string, double>(mostSpentMonth?.Month ?? DateTime.Now.ToString("MMMM"),
                mostSpentMonth?.Amount ?? 0);
        }

        /// <summary>
        /// Obtem o quanto de dinheiro o cliente gastou.
        /// </summary>
        public async Task<double> GetSpentValue()
        {
            return (await GetPayments()).Sum(e => Math.Abs(e.Amount));
        }

        /// <summary>
        /// Obtem o quanto de dinheiro o cliente já recebeu.
        /// </summary>
        public async Task<double> GetReceivedValue()
        {
            return (await GetReceivedPayments()).Sum(e => e.Amount);
        }

        /// <summary>
        /// Obtem o saldo total de movimentações do cliente.
        /// </summary>
        public async Task<double> GetTotalTransactionsAmount()
        {
            return (await GetTransactions()).Sum(e => Math.Abs(e.Amount));
        }
    }
}