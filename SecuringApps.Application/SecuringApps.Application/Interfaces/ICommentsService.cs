using SecuringApps.Application.ViewModels;
using System;
using System.Linq;

namespace SecuringApps.Application.Interfaces
{
    public interface ICommentsService
    {
        IQueryable<CommentsViewModel> GetComments();
        CommentsViewModel GetComments(Guid id);
        void AddComments(CommentsViewModel data);
        void DeleteComments(Guid id);
    }
}
