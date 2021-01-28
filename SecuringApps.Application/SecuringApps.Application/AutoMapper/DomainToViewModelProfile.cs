using AutoMapper;

using System;
using System.Collections.Generic;
using System.Text;

namespace SecuringApps.Application.AutoMapper
{
    public class DomainToViewModelProfile: Profile
    {
        public DomainToViewModelProfile()
        {
          //  CreateMap<Product, ProductViewModel>();
          //  CreateMap<Category, CategoryViewModel>();
            //Product class was used to model the database
            //ProductViewModel class was used to pass on the data to/from the Presentation project/layer
        }

    }
}
