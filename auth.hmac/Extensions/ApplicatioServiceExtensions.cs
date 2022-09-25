

using auth.hmac.Interface;
using auth.hmac.Services;
using auth.models.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace auth.hmac.Extensions
{
    public static class ApplicatioServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services) 
        {
            services.AddScoped<IAuthentication, AuthenticationService>();
            string Constr = "Data Source=localhost;Initial Catalog=authDB;User Id=sa;Password=Abc@12345;Connection Lifetime=30;Pooling=True;Min Pool Size=5;Max Pool Size=100;Connection TimeOut=60;";
            services.AddDbContext<DataContext>(option => option.UseLazyLoadingProxies().UseSqlServer(Constr));
            return services; 
        }
    }
}
