using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BalanceQueryAPI.DAL;

namespace BalanceQueryAPI.Controllers
{
    [RoutePrefix("loanbalance")]
    public class BalanceInquiryController : ApiController
    {
        private readonly DataAccessLayer service = new DataAccessLayer();

        // GET: server/loanbalance/inquiry/12345678912/_v1200
        [Route("inquiry/{account_number}/_v1200")]
        public IHttpActionResult Get(string account_number)
        {
            if (string.IsNullOrWhiteSpace(account_number))
                return BadRequest();
            var balance = service.GetLoanOutstandingBalance(account_number);
            var error = service.ErrorMessage;
            if (balance == null && !string.IsNullOrWhiteSpace(error))
                return InternalServerError(new Exception(error));
            if (balance == null && string.IsNullOrWhiteSpace(error))
                return BadRequest("Account does not exist.");
            return Ok(balance);
        }
    }
}
