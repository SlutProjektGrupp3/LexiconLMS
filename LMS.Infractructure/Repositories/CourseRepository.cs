using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Infractructure.Data;
using LMS.Shared.Request;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

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
}

