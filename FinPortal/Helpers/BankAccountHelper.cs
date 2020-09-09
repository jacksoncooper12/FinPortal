using FinPortal.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinPortal.Helpers
{
    public class BankAccountHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public List<BankAccount> GetMyAccounts()
        {
            var accounts = new List<BankAccount>();
            foreach(var bA in db.BankAccounts)
            {
                if(bA.OwnerId == HttpContext.Current.User.Identity.GetUserId())
                {
                    accounts.Add(bA);
                }
            }
            return accounts;
        }
    }
}