using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Infractructure.Data;
using LMS.Shared.Request;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infractructure.Repositories;

public class CourseRepository : RepositoryBase<Course>, ICourseRepository
{
    public CourseRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Course>> GetAllCoursesAsync(bool trackChanges = false)
    {
        return await FindAll(trackChanges).AsNoTracking().ToListAsync();
    }

    public async Task<Course?> GetCourseByIdAsync(Guid courseId, bool trackChanges = false)
    {
        return await FindByCondition(c => c.Id == courseId, trackChanges)
            .Include(c => c.Modules)
            .FirstOrDefaultAsync();
    }

    public async Task<PagedList<Course>> GetAllCoursesAsync(CourseRequestParams requestParams, bool trackChanges = false)
    {
        IQueryable<Course> courses = requestParams.IncludeStudents ? FindAll(trackChanges).Include(c => c.Students) :
                                                      FindAll(trackChanges);

        return await PagedList<Course>.CreateAsync(courses, requestParams.PageNumber, requestParams.PageSize);
    }

    public void CreateCourse(Course course) => Create(course);

}
