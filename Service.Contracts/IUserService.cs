
using LMS.Shared.DTOs.User;

namespace Service.Contracts;

public interface IUserService
{
    Task<IEnumerable<UserListDto>> GetAllUsersAsync();
    Task DeleteUserAsync(string id);
}
