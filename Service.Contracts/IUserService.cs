
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
}