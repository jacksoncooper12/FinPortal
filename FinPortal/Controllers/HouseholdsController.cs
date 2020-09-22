using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using FinPortal.Extensions;
using FinPortal.Helpers;
using FinPortal.Models;
using FinPortal.ViewModels;
using Microsoft.AspNet.Identity;

namespace FinPortal.Controllers
{
    public class HouseholdsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private RoleHelper roleHelper = new RoleHelper();

        // GET: Households
        public ActionResult Index()
        {
            return View(db.Households.ToList());
        }

        // GET: Households/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Households.Find(id);
            if (household == null)
            {
                return HttpNotFound();
            }
            return View(household);
        }

        // GET: Households/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Households/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "New User")]
        public async Task<ActionResult> Create([Bind(Include = "Id,HouseholdName,Greeting")] Household household)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.Find(User.Identity.GetUserId());
                household.Created = DateTime.Now;
                db.Households.Add(household);
                db.SaveChanges();


                user.HouseholdId = household.Id;
                foreach(var account in user.Accounts)
                {
                    account.HouseholdId = household.Id;
                }
                foreach (var budget in user.Budgets)
                {
                    budget.HouseholdId = household.Id;
                }
                roleHelper.UpdateUserRole(user.Id, "Head");
                db.SaveChanges();

                await AuthorizeExtensions.RefreshAuthentication(HttpContext, user);

                return RedirectToAction("Index", "Home");
            }
            return View(household);
        }

        [HttpPost]
        public ActionResult FindHousehold(string code)
        {
            var realGuid = Guid.Parse(code);
            var invitation = db.Invitations.FirstOrDefault(i => i.Code == realGuid);
            if (invitation == null)
            {
                return View("NotFound", invitation);
            }
            var expirationDate = invitation.Created.AddDays(invitation.TTL);
            if (invitation.IsValid && DateTime.Now < expirationDate)
            {
                return RedirectToAction("Success","Households", new { invitation.Household.Id});
            }
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> JoinHousehold(int Id)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var household = db.Households.Find(Id);
            if (household == null)
            {
                return RedirectToAction("Failed");
            }
            user.HouseholdId = household.Id;
            foreach (var account in user.Accounts)
            {
                account.HouseholdId = household.Id;
            }
            foreach (var budget in user.Budgets)
            {
                budget.HouseholdId = household.Id;
            }
            roleHelper.UpdateUserRole(user.Id, "Member");
            db.SaveChanges();

            await AuthorizeExtensions.RefreshAuthentication(HttpContext, user);
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Failed()
        {
            return View();
        }

        public ActionResult Success(int Id)
        {
            var household = db.Households.Find(Id);
            return View(household);
        }

        public ActionResult ConfigureHousehold()
        {
            var model = new ConfigureHouseVM();
            model.HouseholdId = User.Identity.GetHouseholdId();
            if (model.HouseholdId == null)
            {
                return RedirectToAction("Create");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfigureHousehold(ConfigureHouseVM model)
        {
            var bankAccount = new BankAccount(model.BankAccount.StartingBalance, model.BankAccount.WarningBalance, model.BankAccount.AccountName);
            bankAccount.AccountType = model.BankAccount.AccountType;
            bankAccount.HouseholdId = (int)model.HouseholdId;
            db.BankAccounts.Add(bankAccount);

            db.SaveChanges();

            var budget = new Budget();
            budget.HouseholdId = (int)model.HouseholdId;
            budget.BudgetName = model.Budget.BudgetName;
            db.Budgets.Add(budget);

            db.SaveChanges();

            var budgetItem = new BudgetItem();
            budgetItem.BudgetId = budget.Id;
            budgetItem.TargetAmount = model.BudgetItem.TargetAmount;
            budgetItem.ItemName = model.BudgetItem.ItemName;
            db.BudgetItems.Add(budgetItem);
            db.SaveChanges();

            return RedirectToAction("Index", "Home");
        }
        // GET: Households/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Households.Find(id);
            if (household == null)
            {
                return HttpNotFound();
            }
            return View(household);
        }

        // POST: Households/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,HouseholdName,Greeting,Created,IsDeleted")] Household household)
        {
            if (ModelState.IsValid)
            {
                db.Entry(household).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(household);
        }
        [Authorize(Roles ="Head")]
        public ActionResult ExitDenied()
        {
            return View();
        }
        [Authorize(Roles = "Head")]
        public ActionResult ChangeHead()
        {
            var myHouseId = db.Users.Find(User.Identity.GetUserId()).HouseholdId ?? 0;
            if(myHouseId == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            var members = db.Users.Where(u => u.HouseholdId == myHouseId).ToList();
            ViewBag.NewHoH = new SelectList(members, "Id", "FullName");
            return View();
        }
        [HttpPost, ActionName("ChangeHead")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeHeadAsync(string newHoH, bool leave)
        {
            if (string.IsNullOrEmpty(newHoH))
            {
                return RedirectToAction("Index", "Home");
            }
            var user = db.Users.Find(User.Identity.GetUserId());
            roleHelper.UpdateUserRole(newHoH, "Head");
            if (leave)
            {
                user.Household.IsDeleted = true;
                user.HouseholdId = null;
                foreach (var account in user.Accounts)
                {
                    account.HouseholdId = null;
                }
                foreach (var budget in user.Budgets)
                {
                    budget.HouseholdId = null;
                }
                db.SaveChanges();
                roleHelper.UpdateUserRole(user.Id, "New User");
                await AuthorizeExtensions.RefreshAuthentication(HttpContext, user);
            }
            else
            {
                roleHelper.UpdateUserRole(user.Id, "Member");
                await AuthorizeExtensions.RefreshAuthentication(HttpContext, user);
            }
            return RedirectToAction("Index", "Home");
        }

        public async Task<ActionResult> LeaveAsync()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            var role = roleHelper.ListUserRoles(userId).FirstOrDefault();
            switch (role)
            {
                case "Head":
                    var memberCount = db.Users.Where(g => g.HouseholdId == user.HouseholdId).Count() - 1;
                    if(memberCount >= 1)
                    {
                        TempData["Message"] = $"You are unable to leave the Household at this <br/>time, as there are still {memberCount} other members.";
                        return RedirectToAction("ExitDenied");
                    }
                    user.Household.IsDeleted = true;
                    user.HouseholdId = null;
                    foreach(var account in user.Accounts)
                    {
                        account.HouseholdId = null;
                    }
                    foreach (var budget in user.Budgets)
                    {
                        budget.HouseholdId = null;
                    }
                    //var household = db.Households.Find(user.HouseholdId);
                    //db.Households.Remove(household);
                    roleHelper.UpdateUserRole(userId, "New User");
                    db.SaveChanges();
                    await AuthorizeExtensions.RefreshAuthentication(HttpContext, user);
                    return RedirectToAction("Index","Home");
                case "Member":
                    user.HouseholdId = null;
                    foreach (var account in user.Accounts)
                    {
                        account.HouseholdId = null;
                    }
                    foreach (var budget in user.Budgets)
                    {
                        budget.HouseholdId = null;
                    }
                    roleHelper.UpdateUserRole(userId, "New User");
                    db.SaveChanges();


                    await AuthorizeExtensions.RefreshAuthentication(HttpContext, user);

                    return RedirectToAction("Index", "Home");
                default:
                    return RedirectToAction("Index","Home");
            }
        }

        // GET: Households/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Households.Find(id);
            if (household == null)
            {
                return HttpNotFound();
            }
            return View(household);
        }

        // POST: Households/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Household household = db.Households.Find(id);
            db.Households.Remove(household);
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
