using System.ComponentModel.DataAnnotations;

namespace LMS.Shared.DTOs.User;

public class CreateUserDto
{
    [Required(ErrorMessage = "First name is required.")]
    public string FirstName { get; set; } = "";

    [Required(ErrorMessage = "Last name is required.")]
    public string LastName { get; set; } = "";

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(3, ErrorMessage = "Password must be at least 3 characters.")]
    [RegularExpression(@"^(?!^(.)\1*$).+$",
        ErrorMessage = "Passwords must use at least 2 different characters.")]
    public string Password { get; set; } = "";

    [Required(ErrorMessage = "Confirm password is required.")]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = "";

    [Required(ErrorMessage = "User must have a role.")]
    public string RoleName { get; set; } = "";
}