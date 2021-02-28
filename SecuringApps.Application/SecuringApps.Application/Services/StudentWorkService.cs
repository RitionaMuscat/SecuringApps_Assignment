using AutoMapper;
using AutoMapper.QueryableExtensions;
using SecuringApps.Application.Interfaces;
using SecuringApps.Application.ViewModels;
using SecuringApps.Domain.Interfaces;
using SecuringApps.Domain.Models;
using System;

namespace SecuringApps.Application.Services
{
    public class StudentWorkService : IStudentWorkService
    {
        private IStudentWorkRepository _studentWorkRepo;
        private IMapper _mapper;

        public StudentWorkService(IStudentWorkRepository studentWorkRepo, IMapper mapper)
        {
            _studentWorkRepo = studentWorkRepo;
            _mapper = mapper;
        }
        public void AddStudentWork(StudentWorkViewModel data)
        {
            StudentWork work = new StudentWork();
            work.TaskId = data.StudentTask.Id;
            work.workOwner = data.workOwner;
            work.filePath = data.filePath;
            work.submittedOn = data.submittedOn;
            work.signature = data.signature;
            work.isDigitallySigned = data.isDigitallySigned;
            _studentWorkRepo.AddStudentWork(work);

        }

        public void DeleteStudentWork(Guid id)
        {
            if (_studentWorkRepo.GetStudentWork(id) != null)
                _studentWorkRepo.DeleteStudentWork(id);
        }

        public StudentWorkViewModel GetStudentWork(Guid id)
        {
            StudentWork studentWork = _studentWorkRepo.GetStudentWork(id);
            var resultingStudentWorkViewModel = _mapper.Map<StudentWorkViewModel>(studentWork);
            return resultingStudentWorkViewModel;
        }

        public System.Linq.IQueryable<StudentWorkViewModel> GetStudentWork()
        {
            return _studentWorkRepo.GetStudentWork().ProjectTo<StudentWorkViewModel>(_mapper.ConfigurationProvider);

        }
    }
}
