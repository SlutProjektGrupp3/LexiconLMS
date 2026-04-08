using LMS.Shared.DTOs;
using LMS.Shared.DTOs.Activity;

namespace Service.Contracts;

public interface IActivityService
{
    Task<IEnumerable<ActivityDto>> GetActivitiesForModuleAsync(Guid moduleId);
    Task<ResultDto<ActivityDto>> CreateActivityAsync(CreateActivityDto dto);
    Task<IEnumerable<ActivityTypeDto>> GetAllActivityTypesAsync();
    Task<ResultDto<bool>> DeleteActivityAsync(Guid id);
    Task<ResultDto<ActivityDto>> UpdateActivityAsync(Guid id, UpdateActivityDto dto);

}