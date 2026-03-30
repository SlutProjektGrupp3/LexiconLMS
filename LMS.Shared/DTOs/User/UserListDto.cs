
namespace LMS.Shared.DTOs.User;

public record UserListDto(
    string Id,
    string FirstName,
    string LastName,
    string Email,
    string RoleName
);