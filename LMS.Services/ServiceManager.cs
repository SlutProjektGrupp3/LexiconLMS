using Service.Contracts;

namespace LMS.Services;

public class ServiceManager : IServiceManager
{
    private Lazy<IModulesService> modulesService;
    private Lazy<IAuthService> authService;

    public IModulesService ModulesService => modulesService.Value;
    private Lazy<ICourseService> courseService;
    
    private readonly Lazy<IActivityService> _activityService;
    public IActivityService ActivityService => _activityService.Value;
    public ICourseService CourseService => courseService.Value;
    
    public IAuthService AuthService => authService.Value;

    public ServiceManager(Lazy<IAuthService> authService, Lazy<ICourseService> courseService, Lazy<IActivityService> activityService)
    {
        this.modulesService = modulesService;
        this.courseService = courseService;
        
        this.authService = authService;
        _activityService = activityService;
    }
}
