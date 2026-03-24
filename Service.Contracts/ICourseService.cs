using LMS.Shared.DTOs.Course;
using LMS.Shared.DTOs.CourseDtos;
using LMS.Shared.Request;

namespace Service.Contracts;

public interface ICourseService
{
    public interface ICourseService
    {
        Task<(IEnumerable<CourseDto> courseDtos, MetaData metaData)> GetCoursesAsync(CourseRequestParams requestParams, bool trackChanges = false);
        Task<IEnumerable<CourseDto>> GetAllCoursesAsync (bool trackChanges = false);
        Task<CourseDto> CreateCourseAsync (CourseCreateDto courseCreateDto);
        Task<CourseDetailsDto?> GetCourseByIdAsync(Guid id);
    }
}    
