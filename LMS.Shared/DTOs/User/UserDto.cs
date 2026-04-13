
namespace LMS.Shared.DTOs.User;

// Make all constructor parameters optional so AutoMapper can construct the record
public record UserDto(
    string Id = "",
    string FirstName = "",
    string LastName = "",
    string Email = "",
    string RoleName = "",
    Guid? CourseId = null
);
