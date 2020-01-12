using System;

namespace Interview.Models
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public DateTime PostingDate { get; set; }
        public bool IsCleared { get; set; }
        public DateTime? ClearedDate { get; set; }
        public int ApplicationId { get; set; }
        public TransactionType Type { get; set; }
        public TransactionSummary Summary { get; set; }
        public double Amount { get; set; }
    }

    public enum TransactionSummary
    {
        Payment,
        Refund
    }

    public enum TransactionType
    {
        Credit,
        Debit
    }
}