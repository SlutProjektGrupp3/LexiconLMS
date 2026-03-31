using Domain.Models.Entities;
using LMS.Shared.DTOs.Course;
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
        Task<Course?> GetCourseByIdAsync(Guid courseId, bool trackChanges = false, bool includeModules = false);        
        void CreateCourse(Course course);
        Task<Course> GetCourseWithStudentsAsync(Guid courseId, bool trackChanges);        
    }
}