using System.ComponentModel.DataAnnotations;

namespace LMS.Shared.DTOs.Modules
{
    public class UpdateModuleDto
    {
        public Guid Id { get; set; }
        public Guid CourseId { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }
    }
}