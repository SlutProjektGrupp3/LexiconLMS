using LMS.Shared.DTOs;
using LMS.Shared.DTOs.Course;
using LMS.Shared.DTOs.User;

namespace Service.Contracts;

public interface ICourseService
{
    Task<IEnumerable<CourseDetailsDto>> GetAllCoursesAsync();
    Task<IEnumerable<CourseDto?>> GetActiveCoursesAsync();
    Task<CourseDto> CreateCourseAsync(CreateCourseDto dto);
    Task<CourseDetailsDto?> GetCourseByIdAsync(Guid id);
    Task UpdateCourseAsync(Guid id, UpdateCourseDto updateCourseDto, bool trackChanges);
    Task DeleteCourseAsync(Guid id, bool trackChanges);
    Task AddStudentToCourseAsync(Guid courseId, string studentId);
    Task<IEnumerable<AvailableStudentDto>> GetAvailableStudentsAsync();
    Task<(IEnumerable<CourseDetailsDto> Items, int TotalCount, int TotalActiveCourses)> GetCourseSummariesAsync(string? search = null, bool? active = null, int page = 1, int pageSize = 12);
    Task<IEnumerable<UserDto>> GetParticipantsAsync(Guid courseId);
}    
