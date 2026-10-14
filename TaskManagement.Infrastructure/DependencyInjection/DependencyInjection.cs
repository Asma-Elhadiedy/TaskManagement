
namespace TaskManagement.Infrastructure.DependencyInjection;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddInfrastructureServices(string connectionString)
        {

            services.AddScoped<ISeedData, SeedData>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<IBackgroundServices, BackgroundServices>();

            services.AddDbContext<AppDbContext>(o => 
                o.UseSqlServer(connectionString));

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            }).AddEntityFrameworkStores<AppDbContext>();


            services.AddOptions<JWTTokenOptions>()
                .BindConfiguration(JWTTokenOptions.SectionName);

            services.AddHangfire(configuration =>
            {
                configuration.UseSqlServerStorage(connectionString);
            });
            services.AddHangfireServer();


            return services;
        }
    }
}
