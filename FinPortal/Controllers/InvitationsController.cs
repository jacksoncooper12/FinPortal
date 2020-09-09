using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using FinPortal.Extensions;
using FinPortal.Models;
using Microsoft.AspNet.Identity;

namespace FinPortal.Controllers
{
    public class InvitationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Invitations/Create
        public ActionResult Create()
        {
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "HouseholdName");
            return View();
        }

        // POST: Invitations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(string email, string message)
        {
            var fluffy = new Invitation();
            if (ModelState.IsValid)
            {
                fluffy.RecipientEmail = email;
                fluffy.Code = Guid.NewGuid();
                fluffy.HouseholdId = (int)User.Identity.GetHouseholdId();
                fluffy.Body = message;
                db.Invitations.Add(fluffy);
                db.SaveChanges();
                var trySend = await fluffy.SendInvitation();
                if(trySend == true)
                {
                    return RedirectToAction("Success", "Account");
                }
                else
                {
                    return RedirectToAction("Failed", "Account");
                }
            }
            return View();
        }

        // GET: Invitations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invitation invitation = db.Invitations.Find(id);
            if (invitation == null)
            {
                return HttpNotFound();
            }
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "HouseholdName", invitation.HouseholdId);
            return View(invitation);
        }

        // POST: Invitations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,HouseholdId,Body,IsValid,Created,TTL,RecipientEmail,Code")] Invitation invitation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(invitation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "HouseholdName", invitation.HouseholdId);
            return View(invitation);
        }

        // GET: Invitations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invitation invitation = db.Invitations.Find(id);
            if (invitation == null)
            {
                return HttpNotFound();
            }
            return View(invitation);
        }

        // POST: Invitations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Invitation invitation = db.Invitations.Find(id);
            db.Invitations.Remove(invitation);
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
