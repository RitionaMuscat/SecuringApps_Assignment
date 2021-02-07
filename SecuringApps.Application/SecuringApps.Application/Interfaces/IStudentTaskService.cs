using SecuringApps.Application.ViewModels;
using System;
using System.Linq;

namespace SecuringApps.Application.Interfaces
{
    public interface IStudentTaskService
    {
        StudentTaskViewModel GetStudentTask(Guid id);
        void AddStudentTask(StudentTaskViewModel model);
    }
}
