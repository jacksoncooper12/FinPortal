using FinPortal.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinPortal.Helpers
{
    public class BudgetHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public List<Budget> GetMyBudgets()
        {
            var budgets = new List<Budget>();
            foreach (var bA in db.Budgets)
            {
                if (bA.OwnerId == HttpContext.Current.User.Identity.GetUserId())
                {
                    budgets.Add(bA);
                }
            }
            return budgets;
        }
    }
}
