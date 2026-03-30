using LMS.Shared.DTOs.Modules;

namespace Service.Contracts
{
    public interface IModuleService
    {
        Task<CreateModuleResultDto> CreateModuleAsync(CreateModuleDto createModuleDto);
   
        Task UpdateModuleAsync(Guid moduleId, UpdateModuleDto dto);
    }
}
