using System.ComponentModel.DataAnnotations;

namespace LMS.Shared.DTOs.Module;

public class UpdateModuleDto : BaseModuleDto
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }    
}