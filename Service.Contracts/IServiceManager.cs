namespace Service.Contracts;
public interface IServiceManager
{
    IModulesService ModulesService { get; }

    IAuthService AuthService { get; }
}
