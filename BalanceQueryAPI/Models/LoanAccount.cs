using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BalanceQueryAPI.Models
{
    public class LoanAccount
    {
        public string Account_Number { get; set; }
        public string Account_Name { get; set; }
        public decimal Balance { get; set; }
    }
}