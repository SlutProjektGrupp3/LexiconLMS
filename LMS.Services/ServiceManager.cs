using Service.Contracts;

namespace LMS.Services;

public class ServiceManager : IServiceManager
{
    private Lazy<ICourseService> courseService;
    private Lazy<IAuthService> authService;

    public ICourseService CourseService => courseService.Value;
    public IAuthService AuthService => authService.Value;

    public ServiceManager(Lazy<IAuthService> authService, Lazy<ICourseService> courseService)
    {
        this.courseService = courseService;
        this.authService = authService;        
    }
}
