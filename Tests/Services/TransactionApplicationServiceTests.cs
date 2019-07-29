using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Services;
using Core.Services;
using Data.Transactions.API;
using Data.Transactions.API.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services.Transactions;
using Tests.Gateways;

namespace Tests.Services
{
    [TestClass]
    public class TransactionApplicationServiceTests
    {
        private ITransactionService _testTransactionService;
        private TestApiGateways _testApiGateway;

        //Cria uma instancia toda vez que é chamado, para evitar que os valores sejam armazenados em cache!
        private readonly TransactionApplicationService _applicationService;

        public TransactionApplicationServiceTests()
        {
            _testApiGateway = new TestApiGateways();

            _testTransactionService = new TransactionService(new List<ITransactionReader>
            {
                new TransactionApiReader(_testApiGateway),
            });

            AddSampleTransactions();
            _applicationService = new TransactionApplicationService(_testTransactionService);
        }

        private void AddSampleTransactions()
        {
            _testApiGateway.SetSamplePaymentResponse(new List<ApiResponse>
            {
                new ApiResponse
                {
                    Category = "Animal",
                    Amount = "-11",
                    Date = "11 / Jun"
                },
                new ApiResponse
                {
                    Category = "AniMaL",
                    Amount = "-2",
                    Date = "12 / Mar"
                },
                new ApiResponse
                {
                    Category = "escola",
                    Amount = "-14",
                    Date = "13 /Jun",
                },
            });

            _testApiGateway.SetSamplePaymentReceivedResponse(new List<ApiResponse>
            {
                new ApiResponse
                {
                    Category = "Compras",
                    Amount = "11",
                    Date = "11 / Jun"
                },
                new ApiResponse
                {
                    Category = "Saude",
                    Amount = "5",
                    Date = "12 / Mar"
                },
                new ApiResponse
                {
                    Category = "EsCola",
                    Amount = "14",
                    Date = "13 /Jun",
                }
            });
        }

        [TestMethod]
        public async Task ApplicationService_GetSpentValuesByCategory_IsValid()
        {
            var categories = await _applicationService.GetSpentValuesByCategoryOrdered();
            var expected = new Dictionary<string, double>
            {
                {"Escola", 14},
                {"Animal", 13}
            };

            //Checa se ambas as categorias serão iguais.
            CollectionAssert.AreEqual(expected, categories);
        }

        /// <summary>
        /// Testa se o método GetMostSpentCategory está retornando a categoria que o usuário mais gastou, corretamente.
        /// </summary
        [TestMethod]
        public async Task ApplicationService_GetMostSpentCategory_IsValid()
        {
            var mostSpentCategory = await _applicationService.GetMostSpentCategory();
            var expectedCategory = new KeyValuePair<string, double>("Escola", 14);

            Assert.AreEqual(mostSpentCategory, expectedCategory);
        }

        /// <summary>
        /// Testa se o método GetMostSpentMonth está retornando qual mês o cliente mais gastou, corretamente.
        /// </summary>
        [TestMethod]
        public async Task ApplicationService_GetMostSpentMonth_IsValid()
        {
            var mostSpentMonth = await _applicationService.GetMostSpentMonth();
            var expectedMonth = new KeyValuePair<string, double>("junho", 25);

            Assert.AreEqual(mostSpentMonth, expectedMonth);
        }

        /// <summary>
        /// Testa se o método GetSpentValue está retornando o quanto de dinheiro o cliente gastou, corretamente.
        /// </summary>
        [TestMethod]
        public async Task ApplicationService_GetSpentValue_IsValid()
        {
            var spentValue = await _applicationService.GetSpentValue();
            double expectedSpentValue = 27;

            Assert.AreEqual(spentValue, expectedSpentValue);
        }

        /// <summary>
        /// Testa se o método GetReceivedValue está retornando o quanto de dinheiro o cliente recebeu, corretamente.
        /// </summary>
        [TestMethod]
        public async Task ApplicationService_GetReceivedValue_IsValid()
        {
            var receivedValue = await _applicationService.GetReceivedValue();
            double expectedReceivedValue = 30;

            Assert.AreEqual(receivedValue, expectedReceivedValue);
        }

        /// <summary>
        /// Testa se o método GetTotalTransactionsAmount está retornando o saldo total de movimentações do cliente de forma correta.
        /// </summary>
        [TestMethod]
        public async Task ApplicationService_GetTotalTransactionsAmount_IsValid()
        {
            var receivedValue = await _applicationService.GetTotalTransactionsAmount();
            double expectedReceivedValue = 57;

            Assert.AreEqual(receivedValue, expectedReceivedValue);
        }
    }
}