using FinPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace FinPortal.ViewModels
{
    public class BudgetWizardVM
    {
        public Budget Budget { get; set; }
        public ICollection<BudgetItem> BudgetItems { get; set; }
    }
}