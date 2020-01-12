using System.Collections.Generic;
using Interview.Models;

namespace Interview.Repository
{
    public interface ITransactionsRepository
    {
        List<Transaction> Transactions();
        void SaveChanges();
    }
}
