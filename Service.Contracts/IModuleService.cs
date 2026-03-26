using LMS.Shared.DTOs.Modules;

namespace Service.Contracts
{
    public interface IModuleService
    {
        Task<CreateModuleResultDto> CreateModuleAsync(CreateModuleDto createModuleDto);
    }
}
