using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Web.Http.Results;
using Interview.Controllers;
using Interview.Models;
using Interview.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Interview.Tests.Controllers
{
    public class DummyTransactionsRepository : ITransactionsRepository
    {
        public static Guid TestGuid = Guid.NewGuid();

        private List<Transaction> _transactions = new List<Transaction>
        {
            new Transaction
            {
                Id = TestGuid,
                ApplicationId = 123,
                Type = TransactionType.Credit,
                Summary = TransactionSummary.Payment,
                Amount = 60.0d,
                PostingDate = DateTime.Now,
                IsCleared = false,
                ClearedDate = null
            },
            new Transaction
            {
                Id = Guid.NewGuid(),
                ApplicationId = 234,
                Type = TransactionType.Debit,
                Summary = TransactionSummary.Refund,
                Amount = 102.34d,
                PostingDate = DateTime.Now.AddHours(-1),
                IsCleared = true,
                ClearedDate = DateTime.Now
            }
        };

        public List<Transaction> Transactions()
        {
            return _transactions;
        }

        public void SaveChanges() { }
    }

    [TestClass]
    public class TransactionsControllerTest
    {
        private TransactionsController _controller;

        [TestInitialize]
        public void CreateController()
        {
            _controller = new TransactionsController(new DummyTransactionsRepository());
        }

        [TestMethod]
        public void Get()
        {
            // Arrange
            //done from test initialize

            // Act
            var result = _controller.Get();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<List<Transaction>>));
            Assert.AreEqual(2, ((OkNegotiatedContentResult<List<Transaction>>)result).Content.Count);
        }

        [TestMethod]
        public void GetByIdThatExists()
        {
            // Act
            var result = _controller.Get(DummyTransactionsRepository.TestGuid);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<Transaction>));
        }


        [TestMethod]
        public void GetByIdThatNotExists()
        {
            // Act
            var result = _controller.Get(Guid.NewGuid());

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void PostWithEmptyInput()
        {
            // Act
            var result = _controller.Post(null);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }

        [TestMethod]
        public void PostWithTransactionInput()
        {
            // Act
            var result = _controller.Post(new TransactionBase
            {
                ApplicationId = 999,
                Type = TransactionType.Credit,
                Summary = TransactionSummary.Payment,
                Amount = 96.00d
            });

            // Asserts
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NegotiatedContentResult<Transaction>));
            Assert.IsTrue(((NegotiatedContentResult<Transaction>)result).StatusCode == HttpStatusCode.Accepted);
            var transaction = ((NegotiatedContentResult<Transaction>)result).Content;
            Assert.IsTrue(transaction.Id != Guid.Empty);
            Assert.IsTrue(transaction.PostingDate != DateTime.MinValue);
            Assert.AreEqual(3, ((OkNegotiatedContentResult<List<Transaction>>)_controller.Get()).Content.Count);
        }

        [TestMethod]
        public void PutForExistingTransaction()
        {
            // Act
            var transactionBeforeUpdate =
                ((OkNegotiatedContentResult<Transaction>)_controller.Get(DummyTransactionsRepository.TestGuid)).Content;

            var transactionBeforeUpdateDeepCopy = new Transaction();
            foreach (var pi in typeof(Transaction).GetProperties())
                pi.SetValue(transactionBeforeUpdateDeepCopy, pi.GetValue(transactionBeforeUpdate));

            var result = _controller.Put(DummyTransactionsRepository.TestGuid,
                new TransactionBase
                {
                    Type = TransactionType.Debit,
                    Summary = TransactionSummary.Refund,
                    Amount = 555
                });

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<Transaction>));

            var transactionAfterUpdate = ((OkNegotiatedContentResult<Transaction>)result).Content;

            foreach (var pi in typeof(Transaction).GetProperties(
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                Assert.AreEqual(pi.GetValue(transactionBeforeUpdateDeepCopy), pi.GetValue(transactionAfterUpdate));

            foreach (var pi in typeof(TransactionBase).GetProperties())
                Assert.AreEqual(pi.GetValue(transactionBeforeUpdate), pi.GetValue(transactionAfterUpdate));

            Assert.AreEqual(2, ((OkNegotiatedContentResult<List<Transaction>>)_controller.Get()).Content.Count);
        }

        [TestMethod]
        public void PutForNonExistingTransaction()
        {
            // Act
            var result = _controller.Put(Guid.NewGuid(),
                new TransactionBase
                {
                    Type = TransactionType.Debit,
                    Summary = TransactionSummary.Refund,
                    Amount = 555
                });

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void PutWithEmptyData()
        {
            // Act
            var result = _controller.Put(Guid.NewGuid(), null);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(BadRequestErrorMessageResult));
        }

        [TestMethod]
        public void DeleteExistingTransaction()
        {
            // Act
            var result = _controller.Delete(DummyTransactionsRepository.TestGuid);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkResult));
            var transaction =
                ((OkNegotiatedContentResult<Transaction>)_controller.Get(DummyTransactionsRepository.TestGuid)).Content;
            Assert.IsNotNull(transaction.ClearedDate);
            Assert.IsTrue(transaction.IsCleared);
        }
    }
}
