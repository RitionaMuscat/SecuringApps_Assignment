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

        private bool _isStudent = true;

        public bool isStudent
        {
            get { return _isStudent; }
            set { _isStudent = value; }
        }

    }
}
