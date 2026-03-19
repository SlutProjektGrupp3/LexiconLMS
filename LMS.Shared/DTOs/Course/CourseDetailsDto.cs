
using LMS.Shared.DTOs.Module;

namespace LMS.Shared.DTOs.Course;

public record CourseDetailsDto(
    Guid Id,
    string Name,
    string Description,
    DateTime StartDate,
    DateTime EndDate,
    List<ModuleDto> Modules
);