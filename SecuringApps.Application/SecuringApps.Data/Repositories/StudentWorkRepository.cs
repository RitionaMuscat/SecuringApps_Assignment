using SecuringApps.Data.Context;
using SecuringApps.Domain.Interfaces;
using SecuringApps.Domain.Models;
using System;
using System.Linq;

namespace SecuringApps.Data.Repositories
{
    public class StudentWorkRepository : IStudentWorkRepository
    {
        private SecuringAppsDBContext _context;
        public StudentWorkRepository(SecuringAppsDBContext context)
        {
            _context = context;
        }

        public void AddStudentWork(StudentWork p)
        {
            _context.StudentWork.Add(p);
            _context.SaveChanges();
        }

        public void DeleteStudentWork(Guid id)
        {
            var myStudentWork = GetStudentWork(id);
            _context.StudentWork.Remove(myStudentWork);
            _context.SaveChanges();
        }


        public StudentWork GetStudentWork(Guid id)
        {
            return _context.StudentWork.SingleOrDefault(x => x.Id == id);
            //if it does not find a StudentWork with a matching id...it will return null!!!
        }

        public IQueryable<StudentWork> GetStudentWork()
        {
            return _context.StudentWork;
        }
    }
}
