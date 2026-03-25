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
        private readonly ApplicationDbContext _context;
        public ActivityRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<IEnumerable<ModuleActivity>> GetActivitiesByModuleIdAsync(Guid moduleId, bool trackChanges)
        {
            return await FindByCondition(a => a.ModuleId == moduleId, trackChanges)
                .Include(a => a.Type)
                .ToListAsync();
        }

        public async Task<IEnumerable<ActivityType>> GetAllActivityTypesAsync(bool trackChanges)
        {
            return await _context.ActivityTypes.ToListAsync();
        }
        public async Task<ActivityType?> GetActivityTypeByIdAsync(Guid typeId)
        {
            return await _context.ActivityTypes.FindAsync(typeId);
        }
    }
}
