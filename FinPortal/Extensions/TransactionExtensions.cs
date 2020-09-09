using FinPortal.Enums;
using FinPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinPortal.Extensions
{
    public static class TransactionExtensions
    {
        private static ApplicationDbContext db = new ApplicationDbContext();

        public static void UpdateBalances(this Transaction transaction)
        {
            UpdateBankBalance(transaction);
            if (transaction.TransactionType == TransactionType.Withdrawal)
            {
                UpdateBudgetAmount(transaction);
                UpdateBudgetItemAmount(transaction);
            }
        }
        private static void UpdateBankBalance(Transaction transaction)
        {
            var bankAccount = db.BankAccounts.Find(transaction.AccountId);
            //one-liner
            //bankAccount.CurrentBalance = transaction.TransactionType == TransactionType.Deposit ? bankAccount.CurrentBalance += transaction.Amount : bankAccount.CurrentBalance -= transaction.Amount;

            if (transaction.TransactionType == TransactionType.Deposit)
            {
                bankAccount.CurrentBalance += transaction.Amount;
            }
            else if (transaction.TransactionType == TransactionType.Withdrawal)
            {
                bankAccount.CurrentBalance -= transaction.Amount;
            }
            db.SaveChanges();

        }
        private static void UpdateBudgetAmount(Transaction transaction)
        {
            var budget = db.Budgets.Find(transaction.BudgetItem.BudgetId);
            budget.CurrentAmount += transaction.Amount;
            db.SaveChanges();
        }
        private static void UpdateBudgetItemAmount(Transaction transaction)
        {
            var budgetItem = db.BudgetItems.Find(transaction.BudgetItemId);
            budgetItem.CurrentAmount += transaction.Amount;
            db.SaveChanges();
        }
    }
}