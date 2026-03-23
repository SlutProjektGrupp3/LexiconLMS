
namespace LMS.Shared.DTOs.User;

public record UserListDto(
    string FirstName,
    string LastName,
    string Email,
    string RoleName
);