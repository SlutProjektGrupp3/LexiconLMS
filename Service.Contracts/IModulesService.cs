using LMS.Shared.DTOs.Modules;

namespace Service.Contracts
{
    public interface IModulesService
    {
        Task<CreateModuleResultDto> CreateModuleAsync(CreateModuleDto createModuleDto);
    }
}
