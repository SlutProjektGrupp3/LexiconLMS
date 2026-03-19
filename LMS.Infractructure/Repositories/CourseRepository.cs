using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Infractructure.Data;
using LMS.Shared.Request;
using Microsoft.EntityFrameworkCore;
using Service.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace LMS.Infractructure.Repositories
{
    public class CourseRepository : RepositoryBase<Course>, ICourseRepository
    {
        public CourseRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Course>> GetAllCoursesAsync(bool trackChanges = false)
        {
            return await FindAll(trackChanges).AsNoTracking().ToListAsync();
        }

        public async Task<Course?> GetCourseAsync(Guid id, bool include = false, bool trackChanges = false)
        {
            return include ?
                await FindByCondition(c => c.Id == id, trackChanges).Include(c => c.Students).FirstOrDefaultAsync() :
                await FindByCondition(c => c.Id == id, trackChanges).FirstOrDefaultAsync();
        }        

        public async Task<PagedList<Course>> GetCoursesAsync(CourseRequestParams requestParams, bool trackChanges = false)
        {
            IQueryable<Course> courses = requestParams.IncludeStudents ? FindAll(trackChanges).Include(c => c.Students) :
                                                          FindAll(trackChanges);

            return await PagedList<Course>.CreateAsync(courses, requestParams.PageNumber, requestParams.PageSize);
        }

        public void CreateCourse(Course course) => Create(course);
﻿
using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Infractructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infractructure.Repositories;

public class CourseRepository : RepositoryBase<Course>, ICourseRepository
{
    private readonly ApplicationDbContext context;
    public CourseRepository(ApplicationDbContext context) : base(context)
    {
        this.context = context;
    }
    public async Task<Course?> GetCourseByIdAsync(Guid courseId, bool trackChanges = false)
    {
        return await FindByCondition(c => c.Id == courseId, trackChanges)
            .Include(c => c.Modules)
            .FirstOrDefaultAsync();
    }
}
