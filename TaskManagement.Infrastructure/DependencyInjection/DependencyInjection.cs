using Quartz;
using Hangfire;

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
            services.AddScoped<IBackgroundServices, HangfireBackgroundServices>();
            services.AddScoped<QuartzBackgroundServices>();

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

            services.AddQuartz(q =>
            {
                //o.UsePersistentStore(s => s.UseSqlServer(connectionString));
                var jobKey = new JobKey("TaskCompletionJob", "TasksGroup");

                q.AddJob<QuartzBackgroundServices>(opts => opts.WithIdentity(jobKey));


                q.AddTrigger(opts => opts
                    .ForJob(jobKey)
                    .WithIdentity("TaskCompletionTrigger", "TasksGroup")
                    .StartNow()
                    .WithSimpleSchedule(x => x
                        .WithInterval(TimeSpan.FromMinutes(1))
                        .RepeatForever()));
            });

            services.AddQuartzHostedService(o =>
                o.WaitForJobsToComplete = true);


            return services;
        }
    }
}
