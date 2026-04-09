using LMS.Shared.DTOs.Course;

namespace Service.Contracts;

public interface IStudentService
{
    Task<CourseDetailsDto?> GetMyCourseAsync(Guid userId);
}
