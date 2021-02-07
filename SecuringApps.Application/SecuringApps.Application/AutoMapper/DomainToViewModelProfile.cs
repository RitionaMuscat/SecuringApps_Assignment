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
          //  CreateMap<Category, CategoryViewModel>();
            //Product class was used to model the database
            //ProductViewModel class was used to pass on the data to/from the Presentation project/layer
        }

    }
}
