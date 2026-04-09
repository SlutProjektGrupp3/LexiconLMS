
using LMS.Shared.DTOs.User;

namespace Service.Contracts;

public interface IUserService
{
    Task<List<UserDto>> GetAllUsersAsync();
    Task<CreateUserResultDto> CreateUserAsync(CreateUserDto userCreateDto);
    Task DeleteUserAsync(string id);
    Task<List<string?>> GetAllRolesAsync();
}