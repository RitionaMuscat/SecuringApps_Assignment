using System;

namespace SecuringApps.Application.ViewModels
{
    public  class StudentWorkViewModel
    {
        public Guid Id { get; set; }
        public string filePath { get; set; }

        public string workOwner { get; set; }

        public DateTime submittedOn { get; set; }
        public bool isDigitallySigned { get; set; }

        public string signature { get; set; }

   
        public StudentTaskViewModel StudentTask { get; set; }
    }
}
