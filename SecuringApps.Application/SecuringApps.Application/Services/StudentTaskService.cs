using AutoMapper;
using SecuringApps.Application.Interfaces;
using SecuringApps.Application.ViewModels;
using SecuringApps.Domain.Interfaces;
using SecuringApps.Domain.Models;
using System;

namespace SecuringApps.Application.Services
{
    public class StudentTaskService : IStudentTaskService
    {
        private IStudentTaskRepository _studentTaskRepo;
        private IMapper _autoMapper;
        public StudentTaskService(IStudentTaskRepository studentTaskRepo, IMapper autoMapper)
        {
            _studentTaskRepo = studentTaskRepo;
            _autoMapper = autoMapper;
        }
        public void AddStudentTask(StudentTaskViewModel model)
        {
            _studentTaskRepo.AddStudentTask(_autoMapper.Map<StudentTask>(model));
        }

        public StudentTaskViewModel GetStudentTask(Guid id)
        {
            var p = _studentTaskRepo.GetStudentTask(id);

            if (p == null) return null;
            else
            {
                var result = _autoMapper.Map<StudentTaskViewModel>(p);
                return result;
            }

        }

    }
}

