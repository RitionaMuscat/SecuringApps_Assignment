using AutoMapper;
using SecuringApps.Application.ViewModels;
using SecuringApps.Domain.Models;

namespace SecuringApps.Application.AutoMapper
{
    public class DomainToViewModelProfile: Profile
    {
        public DomainToViewModelProfile()
        {
            CreateMap<StudentTask, StudentTaskViewModel>();
            CreateMap<StudentWork, StudentWorkViewModel>();
            CreateMap<Comments, CommentsViewModel>();
        }

    }
}
