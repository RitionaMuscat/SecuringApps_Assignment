using System;
using System.Collections.Generic;
using System.Text;

namespace SecuringApps.Application.ViewModels
{
   public  class StudentWorkViewModel
    {
        public Guid Id { get; set; }
        public string filePath { get; set; }

        public string workOwner { get; set; }

        public DateTime submittedOn { get; set; }
        public StudentTaskViewModel StudentTask { get; set; }
    }
}
