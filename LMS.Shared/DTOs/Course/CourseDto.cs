
namespace LMS.Shared.DTOs.Course;

public record CourseDto
(
    Guid Id,
    string Name,
    string Description,
    DateTime StartDate,
    DateTime EndDate
);
