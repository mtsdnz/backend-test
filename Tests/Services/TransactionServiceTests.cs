using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Core.Services;
using Data.Transactions;
using Data.Transactions.API;
using Data.Transactions.API.Gateways;
using Data.Transactions.API.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services.Transactions;
using Tests.Gateways;

namespace Tests.Services
{
    [TestClass]
    public class TransactionServiceTests
    {
        private ITransactionService _productionTransactionService;

        //Url da Api de Pagamentos
        private const string ClientPaymentsUrl =
            "https://my-json-server.typicode.com/cairano/backend-test/pagamentos";

        //Url da Api de recebimentos
        private const string ClientReceivedPaymentsUrl =
            "https://my-json-server.typicode.com/cairano/backend-test/recebimentos";

        
        private IApiGateway _productionApiGateway;

        //Diretorio do arquivo transactions.log
        private static readonly string TransactionsLogPath =
            Path.Combine(Environment.CurrentDirectory, "transactions.log");

        public TransactionServiceTests()
        {
            //Inicializa o Gateway da API de testes.
          

            //Inicializa o gateway da API de produção
            _productionApiGateway = new ApiGateway(new ApiConfig(ClientPaymentsUrl, ClientReceivedPaymentsUrl));

           

            _productionTransactionService = new TransactionService(new List<ITransactionReader>
            {
                new TransactionApiReader(_productionApiGateway),
                new TransactionLogReader(TransactionsLogPath)
            });
        }

        [TestMethod]
        public async Task TransactionLogReader_GetTransactions_IsSuccess()
        {
            var transactionsLog = await new TransactionLogReader(TransactionsLogPath).GetTransactions();
            Assert.IsTrue(transactionsLog.IsSuccess, "Could not load Transaction Log File.");

            if (transactionsLog.IsSuccess)
                Assert.IsTrue(transactionsLog.Value.Count > 0, "Transactions log file is empty.");
        }

        [TestMethod]
        public async Task APIGateway_GetClientPayments_IsSuccess()
        {
            var getPayments = await _productionApiGateway.GetClientPayments();
            Assert.IsTrue(getPayments.IsSuccess,
                "Could not get client payments.");
        }

        [TestMethod]
        public async Task APIGateway_GetClientReceivedPayments_IsSuccess()
        {
            var payments = await _productionApiGateway.GetClientReceivedPayments();
            Assert.IsTrue(payments.IsSuccess,
                "Could not get client received payments.");
        }

        [TestMethod]
        public async Task TransactionService_GetTransactions_IsSuccess()
        {
            var result = await _productionTransactionService.GetTransactions();

            Assert.IsTrue(result.IsSuccess, "Get transactions returned false on IsSuccess");
        }


    }
}