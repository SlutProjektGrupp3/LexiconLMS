using LMS.Shared.DTOs.Course;

namespace Service.Contracts;

public interface ICourseService
{
    Task<IEnumerable<CourseDto>> GetAllCoursesAsync(bool trackChanges = false);
    Task<CourseDto> CreateCourseAsync(CreateCourseDto dto);
    Task<CourseDetailsDto?> GetCourseByIdAsync(Guid id);
    Task UpdateCourseAsync(Guid id, UpdateCourseDto updateCourseDto, bool trackChanges);
    Task DeleteCourseAsync(Guid id, bool trackChanges);
    Task<IEnumerable<ParticipantDto>> GetParticipantsAsync(Guid courseId);
}    
