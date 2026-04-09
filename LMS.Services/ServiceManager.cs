using Service.Contracts;

namespace LMS.Services;

public class ServiceManager : IServiceManager
{
    private readonly Lazy<IAuthService> _authService;
    private readonly Lazy<ICourseService> _courseService;
    private readonly Lazy<IModuleService> _moduleService;
    private readonly Lazy<IActivityService> _activityService;
    private readonly Lazy<IUserService> _userService;
    private readonly Lazy<IStudentService> _studentService;

    public IAuthService AuthService => _authService.Value;
    public ICourseService CourseService => _courseService.Value;
    public IModuleService ModuleService => _moduleService.Value;
    public IActivityService ActivityService => _activityService.Value;
    public IUserService UserService => _userService.Value;
    public IStudentService StudentService => _studentService.Value;

    public ServiceManager(Lazy<IAuthService> authService, Lazy<ICourseService> courseService, Lazy<IModuleService> moduleService, Lazy<IActivityService> activityService, Lazy<IUserService> userService, Lazy<IStudentService> studentService)
    {
        _authService = authService;
        _courseService = courseService;
        _moduleService = moduleService;
        _activityService = activityService;
        _userService = userService;
        _studentService = studentService;
    }
}
