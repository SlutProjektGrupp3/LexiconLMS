namespace Service.Contracts;
public interface IServiceManager
{
    IModulesService ModulesService { get; }
    IAuthService AuthService { get; }
    IActivityService ActivityService { get; }
    ICourseService CourseService { get; }

    IAuthService AuthService { get; }
}
