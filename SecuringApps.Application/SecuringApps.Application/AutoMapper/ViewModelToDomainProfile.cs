using AutoMapper;

namespace SecuringApps.Application.AutoMapper
{
    public class ViewModelToDomainProfile:Profile
    {
        public ViewModelToDomainProfile()
        {
          //  CreateMap<ProductViewModel, Product>().ForMember(x=>x.Category, opt => opt.Ignore());
           // CreateMap<CategoryViewModel, Category>();
        }
    }
}
