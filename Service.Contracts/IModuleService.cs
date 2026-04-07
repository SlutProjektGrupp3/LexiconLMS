using LMS.Shared.DTOs.Module;

namespace Service.Contracts;

public interface IModuleService
{
    Task<CreateModuleResultDto> CreateModuleAsync(CreateModuleDto createModuleDto);
    
    Task<DeleteModuleResultDto> DeleteModuleAsync(Guid moduleId);

    Task UpdateModuleAsync(Guid moduleId, UpdateModuleDto dto);
}
