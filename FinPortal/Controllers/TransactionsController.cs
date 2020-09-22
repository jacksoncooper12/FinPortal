 using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FinPortal.Extensions;
using FinPortal.Models;
using Microsoft.AspNet.Identity;

namespace FinPortal.Controllers
{
    public class TransactionsController : Controller
    {
        private FinPortal.Helpers.UserHelper userHelper = new Helpers.UserHelper();
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Transactions
        public ActionResult Index()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var transactions = db.Transactions.Where(g => g.Account.HouseholdId== user.HouseholdId);
            return View(transactions.ToList());
        }

        // GET: Transactions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // GET: Transactions/Create
        public ActionResult Create()
        {
            ViewBag.AccountId = new SelectList(db.BankAccounts, "Id", "AccountName");
            ViewBag.BudgetItemId = new SelectList(db.BudgetItems, "Id", "ItemName");
            ViewBag.OwnerId = new SelectList(db.Users, "Id", "FirstName");
            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int AccountId, int BudgetItemId, [Bind(Include = "Id,TransactionType,Amount,Memo")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.Find(User.Identity.GetUserId());
                transaction.OwnerId = user.Id;
                transaction.Created = DateTime.Now;
                transaction.AccountId = AccountId;
                transaction.BudgetItemId = BudgetItemId;
                db.Transactions.Add(transaction);
                db.SaveChanges();
                var thisT = db.Transactions.Include(t=>t.BudgetItem).FirstOrDefault(t=>t.Id == transaction.Id);
                thisT.UpdateBalances();
                return RedirectToAction("Index", "Home");
            }

            ViewBag.BudgetItemId = new SelectList(db.BudgetItems, "Id", "ItemName", transaction.BudgetItemId);
            ViewBag.OwnerId = new SelectList(db.Users, "Id", "FirstName", transaction.OwnerId);
            return View(transaction);
        }

        // GET: Transactions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            ViewBag.BudgetItemId = new SelectList(db.BudgetItems, "Id", "ItemName", transaction.BudgetItemId);
            ViewBag.OwnerId = new SelectList(db.Users, "Id", "FirstName", transaction.OwnerId);
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AccountId,BudgetItemId,OwnerId,TransactionType,Created,Amount,Memo,IsDeleted")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                var oldTransaction = db.Transactions.AsNoTracking().FirstOrDefault(g=>g.Id == transaction.Id);
                db.Entry(transaction).State = EntityState.Modified;
                db.SaveChanges();
                var newTransaction = db.Transactions.FirstOrDefault(g=>g.Id == transaction.Id);
                newTransaction.EditTransaction(oldTransaction);
                return RedirectToAction("Details", new { transaction.Id});
            }
            ViewBag.BudgetItemId = new SelectList(db.BudgetItems, "Id", "ItemName", transaction.BudgetItemId);
            ViewBag.OwnerId = new SelectList(db.Users, "Id", "FirstName", transaction.OwnerId);
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Transaction transaction = db.Transactions.Find(id);
            transaction.DeleteTransaction();
            db.Transactions.Remove(transaction);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
