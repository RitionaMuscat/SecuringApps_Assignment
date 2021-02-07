using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SecuringApps.Application.AutoMapper;
using SecuringApps.Application.Interfaces;
using SecuringApps.Data;
using SecuringApps.Application.Services;
using SecuringApps.Data.Context;
using SecuringApps.Domain.Interfaces;
using SecuringApps.Data.Repositories;

namespace SecuringApps.IOC
{
    public class DependencyContainer
    {

        //is called when the application (website) starts and does the associations that follow in the method RegisterServices.
        //why?
        //so whenever there's a call that demands an instance of a class (interface) which has been recognized in the method RegisterServices,
        //the RegisterServices method, creates an instance of that on-demand "class"
        //and we are also informing the RegisterServices about the associations that exist between interface - class+implemention

        //what AddScoped mean?

        /*  https://www.tutorialsteacher.com/core/dependency-injection-in-aspnet-core
         *  Singleton: IoC container will create and share a single instance of a service throughout the application's lifetime.
            Transient: The IoC container will create a new instance of the specified service type every time you ask for it.
            Scoped: IoC container will create an instance of the specified service type once per request and will be shared in a single request
         */
        public static void RegisterServices(IServiceCollection services, string connectionString)
        {

            services.AddDbContext<SecuringAppsDBContext>(options =>
              options.UseSqlServer(connectionString
                 ));

            services.AddScoped<IStudentTaskRepository, StudentTaskRepository>();
            services.AddScoped<IStudentTaskService, StudentTaskService>();

            services.AddScoped<IMembersRepository, MembersRepository>();
            services.AddScoped<IMembersService, MemberService>();

            services.AddAutoMapper(typeof(AutoMapperConfiguration));
            AutoMapperConfiguration.RegisterMappings();

        }





    }
}
