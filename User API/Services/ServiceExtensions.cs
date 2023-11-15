using Microsoft.EntityFrameworkCore;
using Repository;

namespace User_API.ServiceExtensions;
public static class ServiceExtensions
    {
        public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration) =>
            services.AddDbContext<RepositoryContext>(opts =>
                opts.UseSqlServer(configuration.GetConnectionString("sqlConnection")));
    }

