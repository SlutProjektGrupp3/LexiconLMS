using Domain.Models.Entities;

namespace Domain.Contracts.Repositories;

public interface IUserRepository
{
    Task <IEnumerable<ApplicationUser>> GetAllUsersAsync(bool trackChanges = false);

    public Task<ApplicationUser?> GetUserByIdAsync(string id, bool trackChanges = false);
}