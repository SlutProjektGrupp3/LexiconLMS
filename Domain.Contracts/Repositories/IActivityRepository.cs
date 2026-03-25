using Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Contracts.Repositories
{
    public interface IActivityRepository
    {
        Task<IEnumerable<ModuleActivity>> GetActivitiesByModuleIdAsync(Guid moduleId, bool trackChanges);
        void Create(ModuleActivity activity);
        Task<IEnumerable<ActivityType>> GetAllActivityTypesAsync(bool trackChanges);
        Task<ActivityType?> GetActivityTypeByIdAsync(Guid typeId);

    }
}
