using LMS.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Contracts
{
    public interface IActivityService
    {
        Task<IEnumerable<ActivityDto>> GetActivitiesForModuleAsync(Guid moduleId);
    }
}
