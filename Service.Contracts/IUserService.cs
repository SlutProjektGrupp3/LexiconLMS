
using LMS.Shared.DTOs;
using LMS.Shared.DTOs.User;

namespace Service.Contracts;

public interface IUserService
{
    Task<List<UserDto>> GetAllUsersAsync();
    Task<ResultDto<UserDto>> CreateUserAsync(CreateUserDto userCreateDto);
    Task<ResultDto<UserDto>> UpdateUserAsync(string id, UpdateUserDto userUpdateDto);
    Task DeleteUserAsync(string id);
    Task<List<string?>> GetAllRolesAsync();
    Task<IEnumerable<UserDto>> GetTeachersAsync();

    Task<UserDto> UpdateUserAsync(string id, UpdateUserDto dto);
    Task<int> GetUsersCountByRoleAsync(string roleName);
    Task<UserDto?> GetUserByIdAsync(string id);
}