using LMS.Shared.DTOs;
using LMS.Shared.DTOs.Activity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Contracts
{
    public interface IActivityService
    {
        Task<IEnumerable<ActivityDto>> GetActivitiesForModuleAsync(Guid moduleId);
        Task<ResultDto<ActivityDto>> CreateActivityAsync(CreateActivityDto dto);
        Task<IEnumerable<ActivityTypeDto>> GetAllActivityTypesAsync();
        Task<ResultDto<bool>> DeleteActivityAsync(Guid id);
        Task<ActivityDto> UpdateActivityAsync(Guid activityId, UpdateActivityDto dto);

    }
}
