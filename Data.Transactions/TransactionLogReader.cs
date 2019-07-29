using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Core.ActionsResult;
using Core.Extensions;
using Core.Helpers;
using Core.Models;
using Core.Services;

namespace Data.Transactions
{
    public class TransactionLogReader : ITransactionReader
    {
        private readonly string _transactionsLogPath;

        public TransactionLogReader(string transactionsLogPath)
        {
            _transactionsLogPath = transactionsLogPath;
        }

        public async Task<Result<List<TransactionData>>> GetTransactions()
        {
            var transactions = new List<TransactionData>();

            if (!File.Exists(_transactionsLogPath))
            {
                return Result<List<TransactionData>>.Fail(null,
                    $"Log de transações não encontrado. Verifique se o arquivo transactions.log se encontra em: {_transactionsLogPath}.");
            }

            var rawTransactions = (await File.ReadAllLinesAsync(_transactionsLogPath)).Skip(1).ToArray();

            foreach (var rawTransaction in rawTransactions)
            {
                //Se a linha estiver vazia, nós pulamos.
                if (string.IsNullOrWhiteSpace(rawTransaction))
                    continue;

                var rawTransactionData = Regex.Split(rawTransaction, @"\s{2,}");

                //Se tivermos menos de 3 items no array, significa que falta dados na transação, então pulamos.
                if (rawTransactionData.Length < 3)
                    continue;

                transactions.Add(ParseTransaction(rawTransactionData));
            }

            return Result<List<TransactionData>>.Ok(transactions);
        }

        private TransactionData ParseTransaction(IReadOnlyList<string> rawTransactionData)
        {
            var date = rawTransactionData[0];
            var description = rawTransactionData[1];
            var amount = rawTransactionData[2];
            var category = rawTransactionData.ElementAtOrDefault(3);

            return new TransactionData
            {
                //faz um parse da data, de acordo com as linguagens informadas(pt-BR, en-US)
                Date = DateHelper.TryParseWithCultures(date.ClearSpaces(), "dd/MMM", new[] {"pt-BR", "en-US"}),
                Description = description,
                Amount = string.IsNullOrEmpty(amount) ? 0 : double.Parse(amount.ClearSpaces(), CultureInfo.GetCultureInfo("pt-BR")),
                Category = string.IsNullOrEmpty(category)
                    ? ""
                    //Usa Title Case no nome da categoria.
                    : CultureInfo.GetCultureInfo("pt-BR").TextInfo.ToTitleCase(category.ToLower().Trim())
            };
        }
    }
}