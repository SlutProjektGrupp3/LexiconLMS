using System.ComponentModel.DataAnnotations;

namespace LMS.Shared.DTOs.Course;

public class CreateCourseDto
{

    [Required]
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    [Required]
    public DateTime StartDate { get; set; }
    [Required]
    public DateTime EndDate { get; set; }
  
}