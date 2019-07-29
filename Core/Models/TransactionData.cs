using System;

namespace Core.Models
{
    public class TransactionData
    {
        /// <summary>
        /// Descrição da transação
        /// </summary>
        public string Description { get; set; }

        public double Amount { get; set; }

        public string Currency { get; set; }

        public string Category { get; set; }

        public DateTime Date { get; set; }
    }
}