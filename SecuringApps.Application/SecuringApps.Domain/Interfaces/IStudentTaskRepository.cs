using SecuringApps.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SecuringApps.Domain.Interfaces
{
    public interface IStudentTaskRepository
    {
        StudentTask GetStudentTask(Guid id);

        IQueryable<StudentTask> GetStudentTask();
        Guid AddStudentTask(StudentTask studentTask);

        void DeleteStudentTask(Guid id);
    }
}
