
namespace LMS.Shared.DTOs.Module;

public record ModuleDto(
    Guid Id,
    string Name,
    string Description,
    DateTime StartDate,
    DateTime EndDate,
    Guid CourseId 
    );
