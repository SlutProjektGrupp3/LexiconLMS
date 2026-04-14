
namespace LMS.Shared.DTOs.Activity;

public record ActivityDto(
    Guid Id,
    string Name,
    string TypeName,
    string Description,
    DateTime StartDate,
    DateTime EndDate,
    Guid TypeId,
    Guid ModuleId
);
