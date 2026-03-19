
using LMS.Shared.DTOs.Course;

namespace Service.Contracts;

public interface ICourseService
{
    Task<CourseDetailsDto?> GetCourseByIdAsync(Guid id);
}
