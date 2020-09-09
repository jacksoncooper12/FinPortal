using FinPortal.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace FinPortal.Extensions
{
    public static class InvitationExtensions
    {
        public static async Task<bool> SendInvitation(this Invitation invitation)
        {
            var Url = new UrlHelper(HttpContext.Current.Request.RequestContext);
            var callbackUrl = Url.Action("AcceptInvitation", "Account", new { recipientEmail = invitation.RecipientEmail, code = invitation.Code }, protocol: HttpContext.Current.Request.Url.Scheme);
            try
            {
                var from = $"Jackson's Financial Portal<{WebConfigurationManager.AppSettings["emailFrom"]}>";
                var emailMessage = new MailMessage(from, invitation.RecipientEmail)
                {
                    Subject = "Household Invitation",
                    Body = invitation.Body + $"<br /><br />Create a new account and join this household by clicking <a href=\"{callbackUrl}\">this link.</a><br /><br />If you already have an account, copy/paste this household code: <h3>{invitation.Code}</h3>",
                    IsBodyHtml = true
                };
                var svc = new EmailService();
                return await svc.SendInviteAsync(emailMessage);
            }
            catch
            {
                return false;
            }

        }
    }
}