using SecuringApps.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecuringApps.Presentation.Models
{
    public class CreateStudentWorkModel
    {
        public List<StudentTaskViewModel> StudentTasks { get; set; }
        public StudentWorkViewModel StudentWork { get; set; }
    }
    public class ListStudentWork
    {
        public List<StudentTaskViewModel> StudentTask { get; set; }
        public List<StudentWorkViewModel> StudentWork { get; set; }


    }
}
