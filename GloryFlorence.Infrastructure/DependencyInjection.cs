using GloryFlorence.Application.Interfaces;
using GloryFlorence.Infrastructure.Data;
using GloryFlorence.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GloryFlorence.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                // SQL Server Configuration (commented out):
                // options.UseSqlServer(connectionString,
                //     b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));

                // PostgreSQL Configuration:
                options.UseNpgsql(connectionString,
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
            });

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<ITokenService, Services.TokenService>();
            services.AddScoped<ICurrentUserService, Services.CurrentUserService>();
            services.AddHttpContextAccessor();

            return services;
        }
    }
}
