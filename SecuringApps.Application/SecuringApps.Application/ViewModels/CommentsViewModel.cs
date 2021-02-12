using System;
using System.Collections.Generic;
using System.Text;

namespace SecuringApps.Application.ViewModels
{
   public  class CommentsViewModel
    {
 
        public Guid Id { get; set; }

        public string comment { get; set; }

        public string writtenBy { get; set; }

        public DateTime writtenOn { get; set; }

        public  StudentWorkViewModel StudentWork { get; set; }

   
    }
}
