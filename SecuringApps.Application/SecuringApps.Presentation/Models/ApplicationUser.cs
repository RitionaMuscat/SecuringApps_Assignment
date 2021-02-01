using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecuringApps.Presentation.Models
{
    public class ApplicationUser : IdentityUser
    {
        private DateTime _LastLoggedIn;

        public DateTime LastLoggedIn
        {
            get { return DateTime.Now; }
            set { _LastLoggedIn = value; }
        }

        //public DateTime LastLoggedIn {get DateTime.Now set Last= DateTime.Now;
        public bool IsStudent { get; set; }

    }
}
