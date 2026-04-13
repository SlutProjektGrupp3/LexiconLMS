using System.ComponentModel.DataAnnotations;

namespace LMS.Shared.DTOs.Activity;

public class UpdateActivityDto : BaseActivityDto
{
    [Required(ErrorMessage = "Select an activity type.")]
    public Guid TypeId { get; set; }
}