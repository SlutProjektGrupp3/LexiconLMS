
namespace LMS.Shared.DTOs.User;

public record UserListDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string RoleName
);