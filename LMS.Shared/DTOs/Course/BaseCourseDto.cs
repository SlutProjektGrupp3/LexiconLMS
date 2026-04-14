using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LMS.Shared.DTOs.Course
{
    public abstract class BaseCourseDto : IValidatableObject
    {
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StartDate == default)
                yield return new ValidationResult("Start date is required.", new[] { nameof(StartDate) });

            if (EndDate == default)
                yield return new ValidationResult("End date is required.", new[] { nameof(EndDate) });

            if (EndDate <= StartDate)
                yield return new ValidationResult("End date must be after Start date.", new[] { nameof(EndDate) });

            if (StartDate < DateTime.Today)
                yield return new ValidationResult("Start date cannot be in the past.", new[] { nameof(StartDate) });
        }
    }
    public class CreateCourseDto : BaseCourseDto { }
    public class UpdateCourseDto : BaseCourseDto { }
}
