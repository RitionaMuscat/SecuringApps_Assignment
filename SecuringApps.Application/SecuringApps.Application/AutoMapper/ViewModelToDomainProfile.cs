using AutoMapper;
using SecuringApps.Application.ViewModels;
using SecuringApps.Domain.Models;

namespace SecuringApps.Application.AutoMapper
{
    public class ViewModelToDomainProfile:Profile
    {
        public ViewModelToDomainProfile()
        {
            CreateMap<StudentWorkViewModel, StudentWork>().ForMember(x=>x.TaskId, opt => opt.Ignore());
            CreateMap<StudentTaskViewModel, StudentTask>();

        }
    }
}
