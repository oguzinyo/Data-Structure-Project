using System;
using System.Collections.Generic;
using System.Text;

namespace BlockchainAnalysis.Models
{
    public class Transaction
    {
        public string TransactionId { get; set; }
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public decimal Amount { get; set; }
        public DateTime Timestamp { get; set; }

        // Max-Heap'te sıralama yapmak için madenci ücreti (Fee) ekliyoruz
        public decimal Fee { get; set; }

        public Transaction(string from, string to, decimal amount, decimal fee)
        {
            TransactionId = Guid.NewGuid().ToString(); // Benzersiz ID
            FromAddress = from;
            ToAddress = to;
            Amount = amount;
            Fee = fee;
            Timestamp = DateTime.UtcNow;
        }
    }
}
