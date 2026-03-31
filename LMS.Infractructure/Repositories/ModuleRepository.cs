using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Infractructure.Data;
using Microsoft.EntityFrameworkCore;
using LMS.Infractructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace LMS.Infractructure.Repositories
{
    public class ModuleRepository : RepositoryBase<Module>, IModuleRepository
    {
        public ModuleRepository(ApplicationDbContext context) : base(context) { }

        public async Task<Module?> GetModuleByIdAsync(Guid moduleId, bool trackChanges = false, bool includeActivities = false)
        {
            var query = FindByCondition(m => m.Id == moduleId, trackChanges);
            
            if (includeActivities)
                query = query.Include(m => m.Activities);
            return await query.SingleOrDefaultAsync();
        }
        
        public async Task<Module?> GetModuleByIdAndCourseIdAsync(Guid moduleId, Guid courseId, bool trackChanges)
      {
          return await FindByCondition(m => m.Id == moduleId && m.CourseId == courseId, trackChanges)
              .FirstOrDefaultAsync();
      }
    }
}



