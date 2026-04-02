using LMS.Shared.DTOs;
using LMS.Shared.DTOs.Course;
using LMS.Shared.DTOs.CourseDtos;
using LMS.Shared.DTOs.User;
using LMS.Shared.Request;

namespace Service.Contracts;

public interface ICourseService
{
    Task<IEnumerable<CourseDto>> GetAllCoursesAsync(bool trackChanges = false);
    Task<ResultDto<CourseDto>> CreateCourseAsync(CreateCourseDto dto);
    Task<CourseDetailsDto?> GetCourseByIdAsync(Guid id);
    Task UpdateCourseAsync(Guid id, UpdateCourseDto updateCourseDto, bool trackChanges);
    Task<ResultDto> DeleteCourseAsync(Guid id, bool trackChanges); 
    Task AddStudentToCourseAsync(Guid courseId, string studentId);
    Task<IEnumerable<AvailableStudentDto>> GetAvailableStudentsAsync();
    Task<IEnumerable<ParticipantDto>> GetParticipantsAsync(Guid courseId);
}    
