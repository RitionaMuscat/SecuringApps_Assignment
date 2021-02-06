using Microsoft.AspNetCore.Identity;
using System;

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

        private bool _isStudent;

        public bool isStudent
        {
            get { return _isStudent; }
            set { _isStudent = value; }
        }

    }
}
