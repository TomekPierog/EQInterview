using System.Collections.Generic;
using System.IO;
using System.Web;
using Interview.Models;
using Newtonsoft.Json;

namespace Interview.Repository
{
    public class TransactionsRepository: ITransactionsRepository
    {
        private List<Transaction> _transactions;
        private readonly string _dataFilePath;

        public TransactionsRepository()
        {
            _dataFilePath = Path.Combine(new DirectoryInfo(HttpRuntime.AppDomainAppPath).Parent.FullName, "data.json");
            if (!File.Exists(_dataFilePath))
                throw new FileNotFoundException("Could not find data file");

            _transactions = JsonConvert.DeserializeObject<List<Transaction>>(
                File.ReadAllText(_dataFilePath));
        }

        public List<Transaction> Transactions() => _transactions;
        public void SaveChanges()
        {
            File.WriteAllText(_dataFilePath, JsonConvert.SerializeObject(_transactions, Formatting.Indented));
        }
    }
}