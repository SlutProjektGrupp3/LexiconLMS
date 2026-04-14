using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Infractructure.Data;
using LMS.Shared.DTOs;
using LMS.Shared.DTOs.Course;
using LMS.Shared.DTOs.Module;
using LMS.Shared.DTOs.User;
using LMS.Shared.Request;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace LMS.Infractructure.Repositories;

public class CourseRepository : RepositoryBase<Course>, ICourseRepository
{
    public CourseRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Course>> GetAllCoursesAsync(bool trackChanges = false)
    {
        return await FindAll(trackChanges).AsNoTracking().ToListAsync();
    }

    public async Task<Course?> GetCourseByIdAsync(Guid courseId, bool trackChanges = false, bool includeModules = false)
    {
        var query = FindByCondition(c => c.Id == courseId, trackChanges);

        if (includeModules)
            query = query.Include(c => c.Modules);

        return await query.SingleOrDefaultAsync();

    }
    public void CreateCourse(Course course)
    {
        Create(course);
    }

    public async Task<PagedList<Course>> GetCoursesAsync(CourseRequestParams requestParams, bool trackChanges)
    {
        IQueryable<Course> courses = requestParams.IncludeStudents ? FindAll(trackChanges).Include(c => c.Students) :
                                                      FindAll(trackChanges);

        return await PagedList<Course>.CreateAsync(courses, requestParams.PageNumber, requestParams.PageSize);
    }

    public async Task<Course?> GetCourseWithStudentsAsync(Guid courseId, bool trackChanges)
    {
        return await FindByCondition(c => c.Id == courseId, trackChanges)
            .Include(c => c.Students)
            .FirstOrDefaultAsync();
    }    

    public async Task<List<CourseDetailsDto>> GetCourseSummariesAsync()
    {
        return await FindAll(false)
            .Select(c => new CourseDetailsDto
            {
                Id = c.Id,
                Name = c.Name,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                ParticipantsCount = c.Students.Count(),
                ModulesCount = c.Modules.Count(),

                Active = DateTime.UtcNow >= c.StartDate &&
                     DateTime.UtcNow <= c.EndDate
            })
            .ToListAsync();
    }

    public async Task<CourseDetailsDto?> GetCourseDetailsAsync(Guid courseId)
    {
        return await FindByCondition(c => c.Id == courseId, false)
            .Select(c => new CourseDetailsDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                StartDate = c.StartDate,
                EndDate = c.EndDate,

                Modules = c.Modules
                    .Select(m => new ModuleDto(
                        m.Id,
                        m.Name,
                        m.Description,
                        m.StartDate,
                        m.EndDate,
                        m.CourseId,
                        new List<LinkDto>()
                    ))
                    .ToList(),

                Participants = c.Students
                    .Select(u => new UserDto(
                        u.Id,
                        u.FirstName,
                        u.LastName,
                        u.Email,
                        "Student",
                        u.Course == null
                            ? null
                            : new CourseDto(
                                u.Course.Id,
                                u.Course.Name,
                                u.Course.Description,
                                u.Course.StartDate,
                                u.Course.EndDate
                            )
                    ))
                    .ToList(),

                ParticipantsCount = c.Students.Count(),
                ModulesCount = c.Modules.Count(),

                Active = DateTime.UtcNow >= c.StartDate &&
                         DateTime.UtcNow <= c.EndDate
            })
            .FirstOrDefaultAsync();
    }

    public async Task<Course?> GetCourseByUserIdAsync(string userId, bool trackChanges = false)
    {
        return await FindByCondition(c => c.Students.Any(s => s.Id == userId), trackChanges)
            .FirstOrDefaultAsync();
    }
    public IQueryable<CourseDetailsDto> GetCourseSummariesQuery()
    {
        return FindAll(false)
        .Select(c => new CourseDetailsDto
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description,
            StartDate = c.StartDate,
            EndDate = c.EndDate,
            ParticipantsCount = c.Students.Count(),
            ModulesCount = c.Modules.Count(),

            Active = DateTime.UtcNow >= c.StartDate &&
                     DateTime.UtcNow <= c.EndDate
        });
    }

    public async Task<IEnumerable<Course?>> GetActiveCoursesAsync(bool trackChanges = false)
    {
        //return active and upcoming courses
        return await FindAll(trackChanges)
            .Where(c => c.EndDate >= DateTime.UtcNow)
            .AsNoTracking()
            .ToListAsync();
    }
}