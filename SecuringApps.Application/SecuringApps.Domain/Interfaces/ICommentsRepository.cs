using SecuringApps.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SecuringApps.Domain.Interfaces
{
    public interface ICommentsRepository
    {
        IQueryable<Comments> GetComments();
        Comments GetComments(Guid id);

        void AddComments(Comments p);

        void DeleteComments(Guid id);
    }
}
