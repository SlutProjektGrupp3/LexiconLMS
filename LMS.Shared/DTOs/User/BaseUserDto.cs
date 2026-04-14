using System.ComponentModel.DataAnnotations;

namespace LMS.Shared.DTOs.User;

public class BaseUserDto : IValidatableObject
{
    [Required(ErrorMessage = "First name is required.")]
    public string FirstName { get; set; } = "";

    [Required(ErrorMessage = "Last name is required.")]
    public string LastName { get; set; } = "";

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "User must have a role.")]
    public string RoleName { get; set; } = "";
    public Guid? CourseId { get; set; } = null;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (RoleName == "Student" && CourseId == null)
        {
            yield return new ValidationResult("Students must be assigned to a course.", new[] { nameof(CourseId) });
        }
    }
}