using System.ComponentModel.DataAnnotations;

namespace LMS.Shared.DTOs.Modules
{
    public record CreateModuleDto
    {
        [Required(ErrorMessage = "Modulen måste ha en namn.")]
        [MaxLength(100, ErrorMessage = "Namn max längden är 100 bokstaver.")]
        [MinLength(2, ErrorMessage = "Namn min längden är 2 bokstaver.")]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Modulen måste ha en start datum.")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Modulen måste ha en slut datum.")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Modulen måste vara koplad till en kurs.")]
        public Guid CourseId { get; set; }
    }
}
