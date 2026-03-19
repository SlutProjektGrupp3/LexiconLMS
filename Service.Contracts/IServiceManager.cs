namespace Service.Contracts;
public interface IServiceManager
{
    IAuthService AuthService { get; }
    IActivityService ActivityService { get; }
    ICourseService CourseService { get; }
}