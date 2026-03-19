using Service.Contracts;

namespace LMS.Services;

public class ServiceManager : IServiceManager
{
    private Lazy<IAuthService> authService;
    private readonly Lazy<IActivityService> _activityService;
    public IAuthService AuthService => authService.Value;
    public IActivityService ActivityService => _activityService.Value;

    public ServiceManager(Lazy<IAuthService> authService, Lazy<IActivityService> activityService)
    {
        this.authService = authService;
        _activityService = activityService; // Add this
    }
}
