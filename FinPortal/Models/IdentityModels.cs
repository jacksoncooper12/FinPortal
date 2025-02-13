﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using FinPortal.Models;

namespace FinPortal.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [Display(Name="First Name")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "First Name must be between 2 and 20 characters")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "First Name must be between 2 and 20 characters")]
        public string LastName { get; set; }

        public int? HouseholdId { get; set; }
        public virtual Household Household { get; set; }
        public string AvatarPath { get; set; }

        [NotMapped]
        public string FullName
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }
        public virtual ICollection<Budget> Budgets { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
        public virtual ICollection<BankAccount> Accounts { get; set; }

        public ApplicationUser()
        {
            Budgets = new HashSet<Budget>();
            Notifications = new HashSet<Notification>();
            Transactions = new HashSet<Transaction>();
            Accounts = new HashSet<BankAccount>();
            AvatarPath = "/Avatars/lightblueuser.png";
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            var hhId = HouseholdId != null ? HouseholdId.ToString(): "";
            var avatarPath = AvatarPath != null ? AvatarPath : "";
            var fullName = FullName != null ? FullName : "";
            userIdentity.AddClaim(new Claim("HouseholdId", hhId));
            userIdentity.AddClaim(new Claim("AvatarPath", avatarPath));
            userIdentity.AddClaim(new Claim("FullName", fullName));

            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
        public DbSet<BudgetItem> BudgetItems { get; set; }

        public DbSet<BankAccount> BankAccounts { get; set; }

        public DbSet<Household> Households { get; set; }

        public DbSet<Budget> Budgets { get; set; }

        public DbSet<Invitation> Invitations { get; set; }

        public DbSet<Notification> Notifications { get; set; }

        public DbSet<Transaction> Transactions { get; set; }
    }
}