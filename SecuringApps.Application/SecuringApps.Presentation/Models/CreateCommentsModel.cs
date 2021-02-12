using SecuringApps.Application.ViewModels;
using System.Collections.Generic;

namespace SecuringApps.Presentation.Models
{
    public class CreateCommentsModel
    {
        public List<StudentWorkViewModel> StudentWork { get; set; }
        public CommentsViewModel Comments { get; set; }
    }
    public class ListComments
    {
        public List<StudentWorkViewModel> StudentWork { get; set; }
        public List<CommentsViewModel> Comments { get; set; }


    }
}
