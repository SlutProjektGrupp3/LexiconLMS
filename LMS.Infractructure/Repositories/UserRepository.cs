
using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Infractructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infractructure.Repositories;

public class UserRepository : RepositoryBase<ApplicationUser>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }
    public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync(bool trackChanges = false)
    {
        return await FindAll(trackChanges)
            .ToListAsync();
    }

    public async Task DeleteUserAsync(Guid id)
    {
        var user = await FindByCondition(u => u.Id == id.ToString(), trackChanges: true).FirstOrDefaultAsync();
        if (user != null)
        {
            Delete(user);
        }
    }
}
