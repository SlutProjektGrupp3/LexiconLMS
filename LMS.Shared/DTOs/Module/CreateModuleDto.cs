using System.ComponentModel.DataAnnotations;

namespace LMS.Shared.DTOs.Module;

public class CreateModuleDto : BaseModuleDto
{
    [Required(ErrorMessage = "The module must be linked to a course.")]
    public Guid CourseId { get; set; }
}
