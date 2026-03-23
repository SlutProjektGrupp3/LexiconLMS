using Service.Contracts;

namespace LMS.Services;

public class ServiceManager : IServiceManager
{
    private Lazy<IAuthService> authService;
    private Lazy<ICourseService> courseService;
    private Lazy<IUserService> userService;
    public IAuthService AuthService => authService.Value;
    public ICourseService CourseService => courseService.Value;
    public IUserService UserService => userService.Value;

    public ServiceManager(Lazy<IAuthService> authService, Lazy<ICourseService> courseService, Lazy<IUserService> userService)
    {
        this.authService = authService;
        this.courseService = courseService;
        this.userService = userService;
    }
}
