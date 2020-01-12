using System.Web.Http;
using Interview.Repository;

namespace Interview.Controllers
{
    public class TransactionsController : ApiController
    {
        private readonly ITransactionsRepository _transactionRepository;

        public TransactionsController(ITransactionsRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        [HttpGet]
        public IHttpActionResult Get()
        {
            return Ok(_transactionRepository.Transactions());
        }
    }
}
