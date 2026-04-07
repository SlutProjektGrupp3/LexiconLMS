using System.ComponentModel.DataAnnotations;

namespace LMS.Shared.DTOs.Module
{
    public record CreateModuleDto
    {
        [Required(ErrorMessage = "The module must have a name.")]
        [MaxLength(100, ErrorMessage = "The maximum length of the name is 100 characters.")]
        [MinLength(2, ErrorMessage = "The minimum length of the name is 2 characters.")]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "The module must have a start date.")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "The module must have an end date.")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "The module must be linked to a course.")]
        public Guid CourseId { get; set; }
    }
}
