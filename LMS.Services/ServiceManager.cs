using Service.Contracts;

namespace LMS.Services;

public class ServiceManager : IServiceManager
{
    private Lazy<IAuthService> authService;
    private Lazy<ICourseService> courseService;
    private readonly Lazy<IActivityService> _activityService;
    public IAuthService AuthService => authService.Value;
    public IActivityService ActivityService => _activityService.Value;
    public ICourseService CourseService => courseService.Value;

    public ServiceManager(Lazy<IAuthService> authService, Lazy<ICourseService> courseService, Lazy<IActivityService> activityService)
    {
        this.authService = authService;
        this.courseService = courseService;
        _activityService = activityService;
    }
}
