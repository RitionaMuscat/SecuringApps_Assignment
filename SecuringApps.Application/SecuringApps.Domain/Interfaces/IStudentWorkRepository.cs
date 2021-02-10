using SecuringApps.Domain.Models;
using System;
using System.Linq;

namespace SecuringApps.Domain.Interfaces
{
    public interface IStudentWorkRepository
    {
        IQueryable<StudentWork> GetStudentWork();
        StudentWork GetStudentWork(Guid id);

        void AddStudentWork(StudentWork p);

        void DeleteStudentWork(Guid id);
    }
}
