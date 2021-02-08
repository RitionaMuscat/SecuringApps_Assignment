using SecuringApps.Application.ViewModels;
using System;
using System.Linq;

namespace SecuringApps.Application.Interfaces
{
    public interface IStudentTaskService
    {
        IQueryable<StudentTaskViewModel> GetStudentTask();
        StudentTaskViewModel GetStudentTask(Guid id);
        void AddStudentTask(StudentTaskViewModel model);
    }
}
