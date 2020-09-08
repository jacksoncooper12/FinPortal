using FinPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinPortal.Helpers
{
    public class HouseholdHelper
    {
        private RoleHelper roleHelper = new RoleHelper();
        private ApplicationDbContext db = new ApplicationDbContext();
        public List<ApplicationUser> ListUsersInHouseholdInRole(int householdId, string roleName)
        {
            var userList = ListUsersInHousehold(householdId);
            var resultList = new List<ApplicationUser>();
            foreach (var user in userList)
            {
                if (roleHelper.IsUserInRole(user.Id, roleName))
                {
                    resultList.Add(user);
                }
            }
            return resultList;
        }
        public ICollection<ApplicationUser> ListUsersInHousehold(int HouseholdId)
        {
            return db.Households.Find(HouseholdId).Members;
        }
    }
}