using Service.Contracts;

namespace LMS.Services;

public class ServiceManager : IServiceManager
{
    private Lazy<IModulesService> modulesService;
    private Lazy<IAuthService> authService;

    public IModulesService ModulesService => modulesService.Value;
    private Lazy<ICourseService> courseService;
    
    public ICourseService CourseService => courseService.Value;
    
    public IAuthService AuthService => authService.Value;

    public ServiceManager(
        Lazy<IModulesService> modulesService,
        Lazy<IAuthService> authService
        )
    public ServiceManager(Lazy<IAuthService> authService, Lazy<ICourseService> courseService)
    {
        this.modulesService = modulesService;
        this.courseService = courseService;
        
        this.authService = authService;
    }
}
