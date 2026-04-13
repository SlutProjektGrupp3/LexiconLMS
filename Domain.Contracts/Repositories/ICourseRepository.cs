using Domain.Models.Entities;
using LMS.Shared.DTOs.Course;
using LMS.Shared.Request;

namespace Domain.Contracts.Repositories;

public interface ICourseRepository : IRepositoryBase<Course>
{
    Task<PagedList<Course>> GetCoursesAsync(CourseRequestParams requestParams, bool trackChanges = false);
    Task<IEnumerable<Course>> GetAllCoursesAsync(bool trackChanges = false);
    Task<IEnumerable<Course?>> GetActiveCoursesAsync(bool trackChanges = false);
    Task<Course?> GetCourseByIdAsync(Guid courseId, bool trackChanges = false, bool includeModules = false);
    void CreateCourse(Course course);
    Task<Course?> GetCourseWithStudentsAsync(Guid courseId, bool trackChanges);
    Task<List<CourseDetailsDto>> GetCourseSummariesAsync();
    Task<CourseDetailsDto?> GetCourseDetailsAsync(Guid courseId);
    Task<Course?> GetCourseByUserIdAsync(string userId, bool trackChanges = false);
}