using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Application.Helpers;
using Application.Services;
using Core.Services;
using Data.Transactions;
using Data.Transactions.API;
using Data.Transactions.API.Gateways;
using Data.Transactions.API.Models;
using Services.Transactions;

namespace Application
{
    class Program
    {
        //Url da Api de Pagamentos
        private const string ClientPaymentsUrl =
            "https://my-json-server.typicode.com/cairano/backend-test/pagamentos";

        //Url da Api de recebimentos
        private const string ClientReceivedPaymentsUrl =
            "https://my-json-server.typicode.com/cairano/backend-test/recebimentos";

        //Diretorio do arquivo transactions.log
        private static readonly string TransactionsLogPath =
            Path.Combine(Environment.CurrentDirectory, "transactions.log");

        static async Task Main(string[] args)
        {
            Console.Title = "Cliente Transações";

            var transactionService = LoadTransactionService();
            var logger = new LoggerService();
            var applicationService = new TransactionApplicationService(transactionService, logger);

            //Exibe as últimas transações
            await applicationService.PrintTransactions();

            //Exibe o total de gastos por categoria
            await applicationService.PrintSpentValuesByCategory();

            //Exibe qual categoria cliente gastou mais;
            await applicationService.PrintMostSpentCategory();

            ConsoleHelper.WriteHeader("Estatisticas de Movimentações");

            //Exibe qual foi o mês que cliente mais gastou
            await applicationService.PrintMostSpentMonth();

            //Exibe quanto de dinheiro o cliente gastou
            await applicationService.PrintSpentValue();

            //Exibe quanto de dinheiro o cliente recebeu
            await applicationService.PrintReceivedValue();

            //Exibe saldo total de movimentações do cliente
            await applicationService.PrintTotalTransactionsAmount();

            Console.ReadKey();
        }

        static ITransactionService LoadTransactionService()
        {
            //Inicializa as configurações da API
            var apiConfig = new ApiConfig(ClientPaymentsUrl, ClientReceivedPaymentsUrl);
            //Inicializa o Gateway da API.
            var apiGateway = new ApiGateway(apiConfig);

            //Inicializa o serviço de transações.
            return new TransactionService(
                new ITransactionReader[]
                {
                    //Responsável por obter as transações da API
                    new TransactionApiReader(apiGateway),
                    //Responsável por obter as transações do transactions.log
                    new TransactionLogReader(TransactionsLogPath)
                });
        }
    }

    public static class ApplicationServiceExtensions
    {
        /// <summary>
        /// Exibe as transações do cliente no Console, de forma ordenada.
        /// </summary>
        public static async Task PrintTransactions(this TransactionApplicationService transactionService)
        {
            var transactions = await transactionService.GetTransactionsOrdered();
            ConsoleHelper.WriteHeader("Últimas transações");
            if (transactions?.Count > 0)
            {
                ConsoleHelper.PrintRow("Data", "Descrição", "Valor", "Categoria");

                foreach (var clientTransaction in transactions)
                {
                    ConsoleHelper.PrintRow(clientTransaction.Date.ToShortDateString(),
                        clientTransaction.Description,
                        clientTransaction.Amount.ToString("N"),
                        clientTransaction.Category);
                }
            }
            else
            {
                Console.WriteLine("Não foram encontradas últimas transações.");
            }
        }

        /// <summary>
        /// Exibe o total de gastos por categoria;
        /// </summary>
        public static async Task PrintSpentValuesByCategory(this TransactionApplicationService transactionService)
        {
            ConsoleHelper.WriteHeader("Total gasto por Categoria");
            var categories = await transactionService.GetSpentValuesByCategoryOrdered();

            if (categories.Count > 0)
            {
                ConsoleHelper.PrintRow("Categoria", "Total Gasto");
                categories.ToList().ForEach(c => ConsoleHelper.PrintRow(c.Key, c.Value.ToString("N")));
            }
            else
            {
                Console.WriteLine("Não há categorias.");
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Informa qual categoria cliente gastou mais;
        /// </summary>
        public static async Task PrintMostSpentCategory(this TransactionApplicationService clientService)
        {
            var category = await clientService.GetMostSpentCategory();
            if (!string.IsNullOrEmpty(category.Key))
            {
                Console.WriteLine(
                    $"A Categoria que você mais gastou foi {category.Key}. Nesta categoria, você gastou um total de R$ {category.Value:N}");
            }
        }

        /// <summary>
        /// Informa qual foi o mês que cliente mais gastou;
        /// </summary>
        public static async Task PrintMostSpentMonth(this TransactionApplicationService clientService)
        {
            var month = await clientService.GetMostSpentMonth();
            Console.WriteLine(
                $"Mês que você mais gastou: {month.Key} (R$ {month.Value:N}).");
        }

        /// <summary>
        /// Informa quanto de dinheiro o cliente gastou.
        /// </summary>
        public static async Task PrintSpentValue(this TransactionApplicationService clientService)
        {
            var spentMoney = await clientService.GetSpentValue();
            Console.WriteLine($"Total Gasto: R$ {spentMoney:N}");
        }

        /// <summary>
        /// Informa quanto de dinheiro o cliente já recebeu.
        /// </summary>
        public static async Task PrintReceivedValue(this TransactionApplicationService clientService)
        {
            var receivedValue = await clientService.GetReceivedValue();
            Console.WriteLine($"Total Recebido: R$ {receivedValue:N}");
        }

        /// <summary>
        /// Informa quanto de dinheiro o cliente já recebeu.
        /// </summary>
        public static async Task PrintTotalTransactionsAmount(this TransactionApplicationService clientService)
        {
            var transactionsAmount = await clientService.GetTotalTransactionsAmount();
            Console.WriteLine($"Total Movimentado: R$ {transactionsAmount:N}");
        }
    }
}