using SecuringApps.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecuringApps.Presentation.Models
{
    public class CreateStudentTaskModel
    {
      public StudentTaskViewModel StudentTask { get; set; }
    }

    public class ListProductModel
    {
        //public List<CategoryViewModel> Categories { get; set; }
        public List<StudentTaskViewModel> StudentTask { get; set; }
    }
}
