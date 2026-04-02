using Service.Contracts;

namespace LMS.Services;

public class ServiceManager : IServiceManager
{
    private readonly Lazy<IAuthService> authService;
    private readonly Lazy<ICourseService> courseService;
    private readonly Lazy<IModuleService> moduleService;
    private readonly Lazy<IActivityService> activityService;
    private readonly Lazy<IUserService> userService;
    private readonly Lazy<IStudentService> studentService;

    public IAuthService AuthService => authService.Value;
    public ICourseService CourseService => courseService.Value;
    public IModuleService ModuleService => moduleService.Value;
    public IActivityService ActivityService => activityService.Value;
    public IUserService UserService => userService.Value;
    public IStudentService StudentService => studentService.Value;

    public ServiceManager(Lazy<IAuthService> authService, Lazy<ICourseService> courseService, Lazy<IModuleService> moduleService, Lazy<IActivityService> activityService, Lazy<IUserService> userService, Lazy<IStudentService> studentService)
    {
        this.authService = authService;
        this.courseService = courseService;
        this.moduleService = moduleService;
        this.activityService = activityService;
        this.userService = userService;
        this.studentService = studentService;
    }
}
