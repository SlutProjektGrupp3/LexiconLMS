namespace Service.Contracts;
public interface IServiceManager
{
    IAuthService AuthService { get; }
    ICourseService CourseService { get; }
    IUserService UserService { get; }
}