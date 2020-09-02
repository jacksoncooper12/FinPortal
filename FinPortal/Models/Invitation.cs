﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FinPortal.Models
{
    public class Invitation
    {
        public int Id { get; set; }
        public int HouseholdId { get; set; }
        public virtual Household Household { get; set; }
        public string Body { get; set; }
        //start true
        public bool IsValid { get; set; }
        public DateTime Created { get; set; }

        //TTL = time to live - number of days invitation is valid
        public int TTL { get; internal set; }

        //if(DateTime.Now > Created.AddDays(TTL)){IsValid = false}
        [Display(Name = "Recipient Email")]
        public string RecipientEmail { get; set; }

        public Guid Code { get; set; }

        public Invitation()
        {
            Created = DateTime.Now;
            IsValid = true;
            TTL = 3;
        }

    }
}