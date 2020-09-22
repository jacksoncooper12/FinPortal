using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinPortal.ViewModels
{
    public class HomeVM
    {
        public FinPortal.Models.BankAccount bankAccount { get; set; }
        public FinPortal.Models.Transaction Transaction { get; set; }
    }
}