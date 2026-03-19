using Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Contracts.Repositories
{
    public interface IActivityRepository
    {
        Task<IEnumerable<ModuleActivity>> GetActivitiesByModuleIdAsync(Guid moduleId, bool trackChanges);

    }
}
