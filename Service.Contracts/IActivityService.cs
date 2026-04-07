using LMS.Shared.DTOs.Activity;

namespace Service.Contracts;

public interface IActivityService
{
    Task<IEnumerable<ActivityDto>> GetActivitiesForModuleAsync(Guid moduleId);
    Task<ActivityDto> CreateActivityAsync(CreateActivityDto dto);
    Task<IEnumerable<ActivityTypeDto>> GetAllActivityTypesAsync();
    Task DeleteActivityAsync(Guid id);
    Task<ActivityDto> UpdateActivityAsync(Guid id, UpdateActivityDto dto);
}
