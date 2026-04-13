using System.ComponentModel.DataAnnotations;

namespace LMS.Shared.DTOs.Activity;

public class CreateActivityDto : BaseActivityDto
{
    [Required(ErrorMessage = "Selecting an activity type is required")]
    public Guid TypeId { get; set; }

    public Guid ModuleId { get; set; }
}