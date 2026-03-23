
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
        //All users with their roles
        return await FindAll(trackChanges)
            .ToListAsync();
    }
}
