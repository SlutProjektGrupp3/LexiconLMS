namespace Service.Contracts;
public interface IServiceManager
{
    IModulesService ModulesService { get; }
    ICourseService CourseService { get; }

    IAuthService AuthService { get; }
}
