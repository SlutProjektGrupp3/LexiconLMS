
namespace LMS.Shared.DTOs.User;

public record UserDto(
    string Id,
    string FirstName,
    string LastName,
    string Email,
    string RoleName
);