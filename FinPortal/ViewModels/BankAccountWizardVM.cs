using FinPortal.Enums;
using FinPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace FinPortal.ViewModels
{
    public class BankAccountWizardVM
    {
        public decimal StartingBalance { get; set; }
        public decimal WarningBalance { get; set; }
        public string AccountName { get; set; }
        public AccountType AccountType { get; set; }
    }
}