
using LMS.Shared.DTOs.Module;
using LMS.Shared.DTOs.Shared;

namespace LMS.Shared.DTOs.Course;

public record CourseDetailsDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }   
    public string Description { get; set; }    
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<ModuleDto> Modules { get; set; }

    public List<LinkDto> Links { get; set; }
}