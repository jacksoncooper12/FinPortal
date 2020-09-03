namespace FinPortal.Migrations
{
    using FinPortal.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<FinPortal.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(FinPortal.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
            #region roles
            var roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(context));
            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }
            if (!context.Roles.Any(r => r.Name == "Head"))
            {
                roleManager.Create(new IdentityRole { Name = "Head" });
            }
            if (!context.Roles.Any(r => r.Name == "Member"))
            {
                roleManager.Create(new IdentityRole { Name = "Member" });
            }
            if (!context.Roles.Any(r => r.Name == "New User"))
            {
                roleManager.Create(new IdentityRole { Name = "New User" });
            }
            #endregion
            #region users
            var userManager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(context));

            if (!context.Users.Any(u => u.Email == "jacksoncooper12@gmail.com"))
                if (!context.Users.Any(u => u.Email == "jacksoncooper12@gmail.com"))
                {
                    userManager.Create(new ApplicationUser()
                    {
                        Email = "jacksoncooper12@gmail.com",
                        UserName = "jacksoncooper12@gmail.com",
                        FirstName = "Jackson",
                        LastName = "Cooper",

                    }, "Coopdawg12!");
                    //Step1 : Grab the Id that was just created by adding the above user
                    var userId = userManager.FindByEmail("jacksoncooper12@gmail.com").Id;
                    //Assing the created user with a secific role
                    userManager.AddToRole(userId, "Admin");
                }


            #endregion
        }
    }
}
