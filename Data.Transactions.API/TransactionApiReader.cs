using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Core.ActionsResult;
using Core.Extensions;
using Core.Models;
using Core.Services;
using Data.Transactions.API.Gateways;
using Data.Transactions.API.Models;
using Newtonsoft.Json;

namespace Data.Transactions.API
{
    public sealed class TransactionApiReader : ITransactionReader
    {
        private readonly IApiGateway _gateway;

        public TransactionApiReader(IApiGateway gateway)
        {
            _gateway = gateway;
        }

        /// <summary>
        /// Obtem todas as transações.
        /// </summary>
        /// <returns>Lista de <see cref="TransactionData"/> com todas as transações.</returns>
        public async Task<Result<List<TransactionData>>> GetTransactions()
        {
            var transactions = new List<TransactionData>();
            var payments = await GetPayments(); //obtem os pagamentos feitos, da API.
            if (!payments.IsSuccess) //se tiver erro ao obter os pagamentos, retorna o resultado fail.
            {
                return Result<List<TransactionData>>.Fail(null, payments.Errors);
            }

            var receivedPayments = await GetReceivedPayments(); //Obtem os pagamentos recebidos, da API.
            if (!receivedPayments.IsSuccess) //se tiver erro ao obter os pagamentos recebidos, retorna o resultado fail.
            {
                return Result<List<TransactionData>>.Fail(null, receivedPayments.Errors);
            }

            //Adiciona os pagamentos a lista de transações
            transactions.AddRange(ParseApiResponse(payments.Value));
            //Adiciona os pagamentos recebidos a lista de transações
            transactions.AddRange(ParseApiResponse(receivedPayments.Value));

            return Result<List<TransactionData>>.Ok(transactions);
        }

        /// <summary>
        /// Converte uma lista de <see cref="ApiResponse"/> para uma lista de <see cref="TransactionData"/>
        /// </summary>
        /// <param name="transactions">Transações recebidas da API</param>
        private List<TransactionData> ParseApiResponse(List<ApiResponse> transactions)
        {
            var transactionsData = new List<TransactionData>();
            foreach (var transaction in transactions)
            {
                var currency = transaction.Currency;
                //Caso o Currency(moeda) esteja nulo, mas o CurrencyWrapper(modeda) não esteja, nós setamos a moeda igual ao CurrencyWrapper.
                if (string.IsNullOrWhiteSpace(currency) && !string.IsNullOrEmpty(transaction.CurrencyWrapper))
                {
                    currency = transaction.CurrencyWrapper;
                }

                transactionsData.Add(new TransactionData
                {
                    //faz um parse da data em pt-BR.
                    Date = DateTime.ParseExact(transaction.Date.ClearSpaces(), "dd/MMM",
                        CultureInfo.GetCultureInfo("pt-BR")),
                    Description = transaction.Description,
                    Currency = currency,
                    Amount = string.IsNullOrEmpty(transaction.Amount)
                        ? 0 //se o valor for nulo, ou vazio, definimos ele igual a 0
                        : double.Parse(transaction.Amount
                            .ClearSpaces(), CultureInfo.GetCultureInfo("pt-BR")), // tira todos os espaços do valor, e então dá parse para Double.
                    Category = string.IsNullOrEmpty(transaction.Category)
                        ? ""
                        //Usa Title Case no nome da categoria.
                        : CultureInfo.GetCultureInfo("pt-BR").TextInfo
                            .ToTitleCase(transaction.Category.ToLower().Trim())
                });
            }

            return transactionsData;
        }


        /// <summary>
        /// Obtem os pagamentos realizados, da API.
        /// </summary>
        private async Task<Result<List<ApiResponse>>> GetPayments()
        {
            var paymentsRequest = await _gateway.GetClientPayments();
            if (!paymentsRequest.IsSuccess)
            {
                return Result<List<ApiResponse>>
                    .Fail(null, $"Error on GetPayments Request: " +
                                string.Join(", ", paymentsRequest.Errors));
            }

            return Result<List<ApiResponse>>.Ok(
                JsonConvert.DeserializeObject<List<ApiResponse>>(paymentsRequest.Value));
        }

        /// <summary>
        /// Obtem os pagamentos recebidos, da API.
        /// </summary>
        private async Task<Result<List<ApiResponse>>> GetReceivedPayments()
        {
            var request = await _gateway.GetClientReceivedPayments();
            if (!request.IsSuccess)
            {
                return Result<List<ApiResponse>>
                    .Fail(null, $"Error on GetReceivedPayments Request: " +
                                string.Join(", ", request.Errors));
            }

            return Result<List<ApiResponse>>.Ok(
                JsonConvert.DeserializeObject<List<ApiResponse>>(request.Value));
        }
    }
}