using FinPortal.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FinPortal.Enums;
using FinPortal.Extensions;

namespace FinPortal.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Authorize]
        public ActionResult Index()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var userId = user.Id;
            ViewBag.AccountId = new SelectList(db.BankAccounts.Where(g => g.OwnerId == userId), "Id", "AccountName");
            ViewBag.BudgetItemId = new SelectList(db.BudgetItems.Where(g => g.Budget.OwnerId == userId), "Id", "ItemName");
            ViewBag.BudgetId = new SelectList(db.Budgets.Where(g => g.OwnerId == user.Id), "Id", "BudgetName");
            var tType = new BankAccount();
            return View();
        }
        public PartialViewResult _LoginPartial()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            return PartialView(user);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}