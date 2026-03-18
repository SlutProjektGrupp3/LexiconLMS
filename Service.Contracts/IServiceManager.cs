namespace Service.Contracts;
public interface IServiceManager
{
    ICourseService CourseService { get; }
    IAuthService AuthService { get; }
}