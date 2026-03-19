using Domain.Models.Entities;
using LMS.Shared.Request;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Contracts.Repositories
{
    public interface ICourseRepository : IRepositoryBase<Course>
    {
        Task<PagedList<Course>> GetCoursesAsync(CourseRequestParams requestParams, bool trackChanges = false);
        Task<IEnumerable<Course>> GetAllCoursesAsync(bool trackChanges = false);
        Task<Course?> GetCourseAsync(Guid id, bool include = false, bool trackChanges = false);
        Task<Course?> GetCourseByIdAsync(Guid courseId, bool trackChanges = false);        
        void CreateCourse(Course course);
    }
}