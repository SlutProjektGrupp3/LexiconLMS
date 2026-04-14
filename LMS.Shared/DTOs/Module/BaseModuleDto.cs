using LMS.Shared.DTOs.Course;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LMS.Shared.DTOs.Module
{
    public abstract class BaseModuleDto : IValidatableObject
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

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EndDate <= StartDate)
            {
                yield return new ValidationResult(
                    "End date must be after start date.",
                    new[] { nameof(EndDate) }
                );
            }

            if (StartDate < DateTime.Today)
            {
                yield return new ValidationResult(
                    "Start date cannot be in the past.",
                    new[] { nameof(StartDate) }
                );
            }
        }        
    }    
}