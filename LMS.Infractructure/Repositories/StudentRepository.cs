using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Infractructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Infractructure.Repositories
{
    public class StudentRepository : RepositoryBase<ApplicationUser>, IStudentRepository
    {
        public StudentRepository(ApplicationDbContext context) : base(context) { }
       
        public async Task<ApplicationUser?> GetStudentWithCourseAsync(Guid userId, bool trackChanges = false)
        {
            return await FindByCondition(u => u.Id == userId.ToString(), trackChanges)
                .Include(u => u.Course)
                .ThenInclude(c => c.Modules)
                .SingleOrDefaultAsync();
        }
    }
}
