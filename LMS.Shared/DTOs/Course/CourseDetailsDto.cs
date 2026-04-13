
using LMS.Shared.DTOs.Module;
using LMS.Shared.DTOs.User;

namespace LMS.Shared.DTOs.Course;

public class CourseDetailsDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    // Full course modules (present on details endpoint). Nullable so summary responses can omit them.
    public List<ModuleDto>? Modules { get; set; }

    // Summary-specific fields (nullable so details endpoint can still populate full lists).
    public int? ParticipantsCount { get; set; }
    public int? ModulesCount { get; set; }
    public bool? Active { get; set; }
    public List<UserDto>? Participants { get; set; }
}