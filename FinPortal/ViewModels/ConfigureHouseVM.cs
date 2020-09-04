using FinPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinPortal.ViewModels
{
    public class ConfigureHouseVM
    {
        public int HouseholdId { get; set; }
        #region One of everything
        //public BankAccount BankAccount { get; set; }
        //public Budget Budget { get; set; }
        //public BudgetItem BudgetItem { get; set; }
        #endregion
        #region multiple options
        //public ICollection<BankAccount> BankAccounts { get; set; }
        //public ICollection<Budget> Budgets { get; set; }
        //public ICollection<BudgetItem> BudgetItems { get; set; }
        #endregion
        public ICollection<BankAccountWizardVM> BankAccounts { get; set; }
        public ICollection<BudgetWizardVM> Budgets { get; set; }

    }
}