using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using Domain.Models.Exceptions;
using LMS.Shared.DTOs.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Service.Contracts;
using System.Data;
using System.Reflection;

namespace LMS.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        var users = await _unitOfWork.UserRepository.GetAllUsersAsync();
        
        var dtoList = new List<UserDto>();

        foreach (var user in users)
        {
            var role = await _userManager.GetRolesAsync(user);

            dtoList.Add(new UserDto
            (
                Id: user.Id,
                FirstName: user.FirstName,
                LastName: user.LastName,
                Email: user.Email,
                RoleName: role.FirstOrDefault()
            ));
        }

        return dtoList;
    }

    public async Task<CreateUserResultDto> CreateUserAsync(CreateUserDto userCreateDto)
    {
        var user = new ApplicationUser
        {
            FirstName = userCreateDto.FirstName,
            LastName = userCreateDto.LastName,
            Email = userCreateDto.Email,
            UserName = userCreateDto.Email
        };

        try
        {
            var createResult = await _userManager.CreateAsync(user, userCreateDto.Password);

            if (!createResult.Succeeded)
            {
                var errors = createResult.Errors
                    .Select(e => new UserError
                    {
                        Code = e.Code,
                        Description = e.Description
                    })
                    .ToList();

                return CreateUserResultDto.Failed(errors);
            }

            var roleResult = await _userManager.AddToRoleAsync(user, userCreateDto.RoleName);

            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);

                var errors = roleResult.Errors
                    .Select(e => new UserError
                    {
                        Code = e.Code,
                        Description = e.Description
                    })
                    .ToList();

                return CreateUserResultDto.Failed(errors);
            }

            var createdUser = new UserDto(
                Id: user.Id,
                FirstName: user.FirstName,
                LastName: user.LastName,
                Email: user.Email!,
                RoleName: userCreateDto.RoleName
            );

            return CreateUserResultDto.SuccessWith(createdUser);
        }
        catch
        {
            var errors = new List<UserError>
        {
            new UserError
            {
                Code = "USER_ERROR:DB",
                Description = "An error occurred while saving the user to the database."
            }
        };

            return CreateUserResultDto.Failed(errors);
        }
    }

    public async Task DeleteUserAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
            throw new NotFoundException($"User with id {id} was not found.");

        var result = await _userManager.DeleteAsync(user);

        if (!result.Succeeded)
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
        await _unitOfWork.CompleteAsync();
    }

    public async Task<List<string?>> GetAllRolesAsync()
    {
        return await _roleManager.Roles.Select(r => r.Name).ToListAsync();
    }
    public async Task<UserDto> UpdateUserAsync(string id, UpdateUserDto dto)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            throw new NotFoundException($"User with ID {id} not found.", "User Not Found");
        }

        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.Email = dto.Email;
        user.UserName = dto.Email; 

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            var errorMsg = string.Join(", ", updateResult.Errors.Select(e => e.Description));
            throw new BadRequestException(errorMsg, "Update failed");
        }

        var currentRoles = await _userManager.GetRolesAsync(user);
        var currentRole = currentRoles.FirstOrDefault();

        if (currentRole != dto.RoleName)
        {
            if (currentRole != null)
            {
                await _userManager.RemoveFromRoleAsync(user, currentRole);
            }

            var roleResult = await _userManager.AddToRoleAsync(user, dto.RoleName);
            if (!roleResult.Succeeded)
            {
                var errorMsg = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                throw new BadRequestException(errorMsg, "Role Update Failed");
            }
        }

        return new UserDto(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email,
            dto.RoleName
        );
    }
    public async Task<int> GetUsersCountByRoleAsync(string roleName)
    {
        if (string.IsNullOrWhiteSpace(roleName))
            return 0;

        var role = await _roleManager.FindByNameAsync(roleName);
        if (role == null)
            return 0;

        var usersInRole = await _userManager.GetUsersInRoleAsync(roleName);
        return usersInRole?.Count ?? 0;
    }
    
    public async Task<IEnumerable<UserDto>> GetTeachersAsync()
{
    var teachers = await _userManager.GetUsersInRoleAsync("Teacher");

    return teachers.Select(t => new UserDto(
        t.Id,
        t.FirstName,
        t.LastName,
        t.Email ?? string.Empty,
        "Teacher"
    ));
}
}

