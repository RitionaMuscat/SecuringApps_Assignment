using Microsoft.EntityFrameworkCore;
using SecuringApps.Data.Context;
using SecuringApps.Domain.Interfaces;
using SecuringApps.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SecuringApps.Data.Repositories
{
    public class StudentTaskRepository : IStudentTaskRepository
    {
        SecuringAppsDBContext _context;
        public StudentTaskRepository(SecuringAppsDBContext context)
        { _context = context; }
        public void AddMember(Member m)
        {
            _context.Members.Add(m);
            _context.SaveChanges();
        }

        public Guid AddStudentTask(StudentTask studentTask)
        {
            _context.StudentTask.Add(studentTask);
            _context.SaveChanges();

            return studentTask.Id;
        }

        public void DeleteStudentTask(Guid id)
        {
            StudentTask studentTask = GetStudentTask(id);
            _context.StudentTask.Remove(studentTask);
            _context.SaveChanges();
        }

        public StudentTask GetStudentTask(Guid id)
        {
            return _context.StudentTask.SingleOrDefault(x => x.Id == id);
        }

        public IQueryable<StudentTask> GetStudentTask()
        {
            return _context.StudentTask;
        }
    }
}
