using LMS.Infractructure.Data;
using LMS.Infractructure.Repositories;
using LMS.Presentation;
using LMS.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.Filters;


namespace LMS.API.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            //ToDo: Restrict access to your BlazorApp only!
            options.AddDefaultPolicy(policy =>
            {
                //..
                //..
                //..
            });

            //Can be used during development
            options.AddPolicy("AllowAll", p =>
               p.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader());
        });
    }

    public static void ConfigureSwagger(this IServiceCollection services) =>
                services.AddEndpointsApiExplorer()
               .AddSwaggerGen(setup =>
               {
                   setup.EnableAnnotations();

                   setup.SwaggerDoc("v1", new OpenApiInfo
                   {
                       Title = "LMS API",
                       Version = "v1"
                   });


                   setup.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
                   {
                       In = ParameterLocation.Header,
                       Description = "Paste in JWT access token",
                       Name = "Authorization",
                       Type = SecuritySchemeType.Http,
                       BearerFormat = "JWT",
                       Scheme = "bearer"
                   });

                   setup.OperationFilter<SecurityRequirementsOperationFilter>(true, "bearer");

               });


    public static void ConfigureControllers(this IServiceCollection services)
    {
        services.AddControllers(opt =>
        {
            opt.ReturnHttpNotAcceptable = true;
            opt.Filters.Add(new ProducesAttribute("application/json"));

        })
                .AddNewtonsoftJson()
                .AddApplicationPart(typeof(AssemblyReference).Assembly);
    }

    public static void ConfigureSql(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("ApplicationDbContext") ?? throw new InvalidOperationException("Connection string 'ApplicationDbContext' not found.")));
    }

    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<IModuleRepository, ModuleRepository>();
        services.AddScoped<IActivityRepository, ActivityRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IStudentRepository, StudentRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped(provider => new Lazy<ICourseRepository>(() =>
            provider.GetRequiredService<ICourseRepository>()));

        services.AddScoped(provider => new Lazy<IModuleRepository>(() =>
            provider.GetRequiredService<IModuleRepository>()));

        services.AddScoped(provider => new Lazy<IActivityRepository>(() =>
            provider.GetRequiredService<IActivityRepository>()));

        services.AddScoped(provider => new Lazy<IUserRepository>(() =>
            provider.GetRequiredService<IUserRepository>()));

        services.AddScoped(provider => new Lazy<IStudentRepository>(() =>
            provider.GetRequiredService<IStudentRepository>()));
    }

    public static void AddServiceLayer(this IServiceCollection services)
    {
        services.AddScoped<ICourseService, CourseService>();
        services.AddScoped<IModuleService, ModulesService>();
        services.AddScoped<IActivityService, ActivityService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IStudentService, StudentService>();

        services.AddScoped<IServiceManager, ServiceManager>();

        services.AddScoped(provider => new Lazy<ICourseService>(() =>
            provider.GetRequiredService<ICourseService>()));

        services.AddScoped(provider => new Lazy<IModuleService>(() =>
            provider.GetRequiredService<IModuleService>()));

        services.AddScoped(provider => new Lazy<IActivityService>(() =>
            provider.GetRequiredService<IActivityService>()));

        services.AddScoped(provider => new Lazy<IAuthService>(() =>
            provider.GetRequiredService<IAuthService>()));

        services.AddScoped(provider => new Lazy<IUserService>(() =>
            provider.GetRequiredService<IUserService>()));

        services.AddScoped(provider => new Lazy<IStudentService>(() =>
            provider.GetRequiredService<IStudentService>()));
    }
}
