using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Infractructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Infractructure.Repositories
{
    public class ActivityRepository : RepositoryBase<ModuleActivity>, IActivityRepository
    {
        public ActivityRepository(ApplicationDbContext context) : base(context)
        {
        }
        public async Task<IEnumerable<ModuleActivity>> GetActivitiesByModuleIdAsync(Guid moduleId, bool trackChanges)
        {
            return await FindByCondition(a => a.ModuleId == moduleId, trackChanges)
                .Include(a => a.Type)
                .ToListAsync();
        }
    }
}
