using FinPortal.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinPortal.Helpers
{
    public class UserHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ApplicationUser CurrentUser()
        {
            var userId = HttpContext.Current.User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            return user;
        }
        public string GetFullName(string userId)
        {
            if (userId == null)
            {
                return ("Hello");
            }
            var user = db.Users.Find(userId);
            return user.FullName;
        }
        public string GetUserRole()
        {
            var userId = HttpContext.Current.User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            var roleId = user.Roles.Where(u => u.UserId == userId);
            return roleId.FirstOrDefault().ToString();
        }
        public string GetUserRoleDos(string userId)
        {
            var user = db.Users.Find(userId);
            var roleId = user.Roles.FirstOrDefault();
            return roleId.ToString();
        }
        public string GetAvatarPath()
        {
            var userId = HttpContext.Current.User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            if (user != null)
            {
                return user.AvatarPath;
            }
            else
            {
                return null;
            }
        }
    }
}