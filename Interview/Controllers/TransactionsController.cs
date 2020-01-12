using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using Interview.Models;
using Interview.Repository;

namespace Interview.Controllers
{
    public class TransactionsController : ApiController
    {
        private readonly ITransactionsRepository _transactionRepository;
        private List<Transaction> _transactions;

        public TransactionsController(ITransactionsRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
            _transactions = _transactionRepository.Transactions();
        }

        /// <summary>
        /// Provides all transactions
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult Get()
        {
            return Ok(_transactions);
        }

        /// <summary>
        /// Provides specific transaction
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult Get([FromUri] Guid id)
        {
            var transaction = _transactions.FirstOrDefault(x => x.Id == id);
            if (transaction == null)
                return NotFound();

            return Ok(transaction);
        }

        /// <summary>
        /// Adds new tranaction
        /// </summary>
        /// <param name="transactionBase"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult Post([FromBody] TransactionBase transactionBase)
        {
            if (transactionBase == null)
                return BadRequest();

            var transaction = new Transaction(transactionBase)
            {
                Id = Guid.NewGuid(),
                PostingDate = DateTime.Now
            };

            _transactions.Add(transaction);
            _transactionRepository.SaveChanges();
            return Content(HttpStatusCode.Accepted, transaction);
        }

        /// <summary>
        /// Updates specific transaction
        /// </summary>
        /// <param name="id"></param>
        /// <param name="transactionBase"></param>
        /// <returns></returns>
        [HttpPut]
        public IHttpActionResult Put([FromUri] Guid id, [FromBody]TransactionBase transactionBase)
        {
            if (transactionBase == null)
                return BadRequest("Provided transaction is empty");

            var existingTransaction = _transactions.FirstOrDefault(x => x.Id == id);
            if (existingTransaction == null)
                return NotFound();

            existingTransaction.CopyFromBase(transactionBase);
            _transactionRepository.SaveChanges();
            return Ok(existingTransaction);
        }

        /// <summary>
        /// Removes specific transaction
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public IHttpActionResult Delete([FromUri] Guid id)
        {
            var transactionToBeRemoved = _transactions.FirstOrDefault(x => x.Id == id);
            if (transactionToBeRemoved == null) return NotFound();

            //_transactions.Remove(transactionToBeRemoved); //not sure if it had to be removed or just 'cleared'
            transactionToBeRemoved.ClearedDate = DateTime.Now;
            transactionToBeRemoved.IsCleared = true;

            return Ok();
        }
    }
}
