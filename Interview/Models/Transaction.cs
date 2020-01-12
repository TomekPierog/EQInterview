using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Interview.Models
{
    public class Transaction: TransactionBase
    {
        public Transaction() { }
        public Transaction(TransactionBase transactionBase)
        {
            this.CopyFromBase(transactionBase);
        }

        public Guid Id { get; set; }
        public DateTime PostingDate { get; set; }
        public bool IsCleared { get; set; }
        public DateTime? ClearedDate { get; set; }
    }

    public class TransactionBase
    {
        public int ApplicationId { get; set; }
        public TransactionType Type { get; set; }
        public TransactionSummary Summary { get; set; }
        public double Amount { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum TransactionSummary
    {
        Payment,
        Refund
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum TransactionType
    {
        Credit,
        Debit
    }

    public static class TransactionExtensions
    {
        public static void CopyFromBase(this Transaction transaction, TransactionBase transactionBase)
        {
            foreach (var pi in typeof(TransactionBase).GetProperties())
                pi.SetValue(transaction, pi.GetValue(transactionBase));
        }
    }
}