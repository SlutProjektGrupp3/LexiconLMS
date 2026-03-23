using LMS.API.Extensions;
using LMS.API.Services;
using LMS.Infractructure.Data;
using Microsoft.OpenApi;

namespace LMS.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.ConfigureSql(builder.Configuration);
        builder.Services.ConfigureControllers();

        builder.Services.AddRepositories();
        builder.Services.AddServiceLayer();

        builder.Services.ConfigureAuthentication(builder.Configuration);
        builder.Services.ConfigureIdentity();

        builder.Services.AddHostedService<DataSeedHostingService>();
        builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MapperProfile>());
        builder.Services.ConfigureCors();
        builder.Services.ConfigureSwagger();


        var app = builder.Build();


        // Configure the HTTP request pipeline.
        app.ConfigureExceptionHandler();

        //if (app.Environment.IsDevelopment())
        //{
            app.UseSwagger();
            app.UseSwaggerUI(opt =>
            {
                opt.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            });
        //}

        app.UseHttpsRedirection();
        app.UseCors("AllowAll"); 

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
