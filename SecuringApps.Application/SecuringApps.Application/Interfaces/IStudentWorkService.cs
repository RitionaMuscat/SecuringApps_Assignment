using SecuringApps.Application.ViewModels;
using System;
using System.Linq;

namespace SecuringApps.Application.Interfaces
{
    public interface IStudentWorkService
    {
        IQueryable<StudentWorkViewModel> GetStudentWork();
        StudentWorkViewModel GetStudentWork(Guid id);

        void AddStudentWork(StudentWorkViewModel data);

        void DeleteStudentWork(Guid id);
    }
}
