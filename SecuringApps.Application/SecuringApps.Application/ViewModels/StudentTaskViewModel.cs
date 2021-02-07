using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SecuringApps.Application.ViewModels
{
    public class StudentTaskViewModel
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Please input name of Task")]
        public string Name { get; set; }
        public string Description { get; set; }
        public string FilePath { get; set; }
        public DateTime Deadline { get; set; }

    }
}
