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
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped(provider => new Lazy<ICourseRepository>(() => provider.GetRequiredService<ICourseRepository>()));
    }

    public static void AddServiceLayer(this IServiceCollection services)
    {
        services.AddScoped<IServiceManager, ServiceManager>();

        services.AddScoped<ICourseService, CourseService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped(provider => new Lazy<ICourseService>(() => provider.GetRequiredService<ICourseService>()));

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped(provider => new Lazy<IAuthService>(() => provider.GetRequiredService<IAuthService>()));
    }
}
