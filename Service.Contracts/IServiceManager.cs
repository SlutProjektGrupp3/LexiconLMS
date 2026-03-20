namespace Service.Contracts;
public interface IServiceManager
{
    ICourseService CourseService { get; }
    IModulesService ModulesService { get; }
    IAuthService AuthService { get; }
    IActivityService ActivityService { get; }
    ICourseService CourseService { get; }

    IAuthService AuthService { get; }
}
