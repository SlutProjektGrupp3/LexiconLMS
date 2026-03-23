using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Shared.DTOs.User;
using Microsoft.AspNetCore.Identity;
using Service.Contracts;

namespace LMS.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _userManager = userManager;
    }
    public async Task<IEnumerable<UserListDto>> GetAllUsersAsync()
    {
        var users = await _unitOfWork.UserRepository.GetAllUsersAsync();
        
        var dtoList = new List<UserListDto>();

        foreach (var user in users)
        {
            var role = await _userManager.GetRolesAsync(user);

            dtoList.Add(new UserListDto
            (
                FirstName: user.FirstName,
                LastName: user.LastName,
                Email: user.Email,
                RoleName: role.FirstOrDefault()
            ));
        }

        return dtoList;
    }
}

