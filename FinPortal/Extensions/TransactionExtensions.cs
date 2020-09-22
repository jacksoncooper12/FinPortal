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
        public static void DeleteTransaction(this Transaction transaction)
        {
            var budgetItem = db.BudgetItems.Find(transaction.BudgetItemId);
            var budget = db.Budgets.Find(budgetItem.BudgetId);
            var bankAccount = db.BankAccounts.Find(transaction.AccountId);
            if (transaction.TransactionType == TransactionType.Deposit)
            {
                bankAccount.CurrentBalance -= transaction.Amount;
            }
            if (transaction.TransactionType == TransactionType.Withdrawal)
            {
                bankAccount.CurrentBalance += transaction.Amount;
                budget.CurrentAmount -= transaction.Amount;
                budgetItem.CurrentAmount -= transaction.Amount;
            }
            db.SaveChanges();
        }

        public static void EditTransaction(this Transaction newTransaction, Transaction oldTransaction)
        {
            var budgetItem = db.BudgetItems.Find(newTransaction.BudgetItemId);
            var budget = db.Budgets.Find(budgetItem.BudgetId);
            var bankAccount = db.BankAccounts.Find(newTransaction.AccountId);
            if (oldTransaction.Amount != newTransaction.Amount || oldTransaction.TransactionType != newTransaction.TransactionType)
            {
                if (oldTransaction.Amount != newTransaction.Amount)
                {
                    if (oldTransaction.TransactionType == TransactionType.Withdrawal && newTransaction.TransactionType == TransactionType.Deposit)
                    {
                        bankAccount.CurrentBalance += oldTransaction.Amount + newTransaction.Amount;
                    }
                    if (oldTransaction.TransactionType == TransactionType.Deposit && newTransaction.TransactionType == TransactionType.Withdrawal)
                    {
                        bankAccount.CurrentBalance -= oldTransaction.Amount + newTransaction.Amount;
                    }
                    else
                    {
                        if (newTransaction.TransactionType == TransactionType.Withdrawal)
                        {
                            var diff = newTransaction.Amount - oldTransaction.Amount;
                            bankAccount.CurrentBalance -= diff;
                        }
                        if (newTransaction.TransactionType == TransactionType.Deposit)
                        {
                            var diff = newTransaction.Amount - oldTransaction.Amount;
                            bankAccount.CurrentBalance += diff;
                        }
                    }
                }
                if (oldTransaction.TransactionType == TransactionType.Withdrawal && newTransaction.TransactionType == TransactionType.Deposit)
                {
                    budget.CurrentAmount -= oldTransaction.Amount;
                    budgetItem.CurrentAmount -= oldTransaction.Amount;
                }
                if (oldTransaction.TransactionType == TransactionType.Deposit && newTransaction.TransactionType == TransactionType.Withdrawal)
                {
                    budget.CurrentAmount += newTransaction.Amount;
                    budgetItem.CurrentAmount += newTransaction.Amount;
                }
                else
                {
                    if (newTransaction.TransactionType == TransactionType.Withdrawal)
                    {
                        var diff = newTransaction.Amount - oldTransaction.Amount;
                        budget.CurrentAmount += diff;
                        budgetItem.CurrentAmount += diff;
                    }
                }
                db.SaveChanges();
            }
            var account = db.BankAccounts.Find(newTransaction.AccountId);
            if (account.CurrentBalance < 0)
            {
                var not = new Notification();
                not.RecipientId = account.OwnerId;
                not.Body = $"You have overdrafted account '{account.AccountName}'";
                not.Created = DateTime.Now;
                not.HouseholdId = (int)account.HouseholdId;
                db.Notifications.Add(not);
                db.SaveChanges();
            }
            if (account.CurrentBalance < account.WarningBalance)
            {
                var not = new Notification();
                not.RecipientId = account.OwnerId;
                not.Body = $"Your account '{account.AccountName}' has fallen below the warning level";
                not.Created = DateTime.Now;
                not.HouseholdId = (int)account.HouseholdId;
                db.Notifications.Add(not);
                db.SaveChanges();
            }
        }
        public static void UpdateBalances(this Transaction transaction)
        {
            UpdateBankBalance(transaction);
            if (transaction.TransactionType == TransactionType.Withdrawal)
            {
                UpdateBudgetAmount(transaction);
                UpdateBudgetItemAmount(transaction);
            }
            var account = db.BankAccounts.Find(transaction.AccountId);
            if (account.CurrentBalance < 0)
            {
                var not = new Notification();
                not.RecipientId = account.OwnerId;
                not.Body = $"You have overdrafted account '{account.AccountName}'";
                not.Created = DateTime.Now;
                not.HouseholdId = (int)account.HouseholdId;
                db.Notifications.Add(not);
                db.SaveChanges();
            }
            if (account.CurrentBalance < account.WarningBalance)
            {
                var not = new Notification();
                not.RecipientId = account.OwnerId;
                not.Body = $"Your account '{account.AccountName}' has fallen below the warning level";
                not.Created = DateTime.Now;
                not.HouseholdId = (int)account.HouseholdId;
                db.Notifications.Add(not);
                db.SaveChanges();
            }
        }
        private static void UpdateBankBalance(Transaction transaction)
        {
            var bankAccount = db.BankAccounts.Find(transaction.AccountId);

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
            var budgetItem = db.BudgetItems.Find(transaction.BudgetItemId);
            var budget = db.Budgets.Find(budgetItem.BudgetId);
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