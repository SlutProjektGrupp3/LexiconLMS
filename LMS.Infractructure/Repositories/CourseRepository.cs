
using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Infractructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infractructure.Repositories;

public class CourseRepository : RepositoryBase<Course>, ICourseRepository
{
    public CourseRepository(ApplicationDbContext context) : base(context)
    {
    }
    public async Task<Course?> GetCourseByIdAsync(Guid courseId, bool trackChanges = false)
    {
        return await FindByCondition(c => c.Id == courseId, trackChanges)
            .Include(c => c.Modules)
            .FirstOrDefaultAsync();
    }
}
