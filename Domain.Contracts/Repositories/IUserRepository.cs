using Domain.Models.Entities;

namespace Domain.Contracts.Repositories;

public interface IUserRepository
{
    Task <IEnumerable<ApplicationUser>> GetAllUsersAsync(bool trackChanges = false);
    Task DeleteUserAsync(Guid id);
}