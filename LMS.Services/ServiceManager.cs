using Service.Contracts;

namespace LMS.Services;

public class ServiceManager : IServiceManager
{
    private Lazy<IAuthService> authService;
    private Lazy<ICourseService> courseService;
    public IAuthService AuthService => authService.Value;
    public ICourseService CourseService => courseService.Value;

    public ServiceManager(Lazy<IAuthService> authService, Lazy<ICourseService> courseService)
    {
        this.authService = authService;
        this.courseService = courseService;
    }
}
