using SecuringApps.Data.Context;
using SecuringApps.Domain.Interfaces;
using SecuringApps.Domain.Models;
using System;
using System.Linq;

namespace SecuringApps.Data.Repositories
{
    public class CommentsRepository : ICommentsRepository
    {
        private SecuringAppsDBContext _context;
        public CommentsRepository(SecuringAppsDBContext context)
        {
            _context = context;
        }

        public void AddComments(Comments p)
        {
            _context.Comments.Add(p);
            _context.SaveChanges();
        }

        public void DeleteComments(Guid id)
        {
            var myComments = GetComments(id);
            _context.Comments.Remove(myComments);
            _context.SaveChanges();
        }


        public Comments GetComments(Guid id)
        {
            return _context.Comments.SingleOrDefault(x => x.Id == id);
            //if it does not find a StudentWork with a matching id...it will return null!!!
        }

        public IQueryable<Comments> GetComments()
        {
            return _context.Comments;
        }
    }
}
