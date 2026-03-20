namespace Service.Contracts;
public interface IServiceManager
{
    ICourseService CourseService { get; }
    IModulesService ModulesService { get; }
    IActivityService ActivityService { get; }

    IAuthService AuthService { get; }
}
