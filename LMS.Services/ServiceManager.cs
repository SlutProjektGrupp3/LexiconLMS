using Service.Contracts;

namespace LMS.Services;

public class ServiceManager : IServiceManager
{
    private Lazy<IModulesService> modulesService;
    private Lazy<IAuthService> authService;

    public IModulesService ModulesService => modulesService.Value;
    public IAuthService AuthService => authService.Value;

    public ServiceManager(
        Lazy<IModulesService> modulesService,
        Lazy<IAuthService> authService
        )
    {
        this.modulesService = modulesService;
        this.authService = authService;
    }
}
