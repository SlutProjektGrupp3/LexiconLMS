using System.ComponentModel.DataAnnotations;

namespace LMS.Shared.DTOs.Activity;

public record CreateActivityDto
{
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    [Required]
    public DateTime StartDate { get; set; } = DateTime.Now;

    [Required]
    public DateTime EndDate { get; set; } = DateTime.Now.AddDays(1);

    [Required(ErrorMessage = "Selecting an activity type is required")]
    public Guid? TypeId { get; set; }

    public Guid ModuleId { get; set; }
}