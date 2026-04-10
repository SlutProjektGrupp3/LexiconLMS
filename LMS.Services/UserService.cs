using AutoMapper;
using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Shared.DTOs.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Service.Contracts;
using System.Data;

namespace LMS.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork unitOfWork;
    private readonly UserManager<ApplicationUser> userManager;
    private readonly RoleManager<IdentityRole> roleManager;

    public UserService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        this.userManager = userManager;
        this.roleManager = roleManager;
    }

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        var users = await unitOfWork.UserRepository.GetAllUsersAsync();
        
        var dtoList = new List<UserDto>();

        foreach (var user in users)
        {
            var role = await userManager.GetRolesAsync(user);

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
            var createResult = await userManager.CreateAsync(user, userCreateDto.Password);

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

            var roleResult = await userManager.AddToRoleAsync(user, userCreateDto.RoleName);

            if (!roleResult.Succeeded)
            {
                await userManager.DeleteAsync(user);

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
        var user = await userManager.FindByIdAsync(id);

        if (user == null)
            throw new KeyNotFoundException("User not found.");

        var result = await userManager.DeleteAsync(user);

        if (!result.Succeeded)
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
    }

    public async Task<List<string?>> GetAllRolesAsync()
    {
        return await roleManager.Roles.Select(r => r.Name).ToListAsync();
    }

    public async Task<IEnumerable<UserDto>> GetTeachersAsync()
    {
        var teachers = await userManager.GetUsersInRoleAsync("Teacher");

        return teachers.Select(t => new UserDto(
            t.Id,
            t.FirstName,
            t.LastName,
            t.Email ?? string.Empty,
            "Teacher"
        ));
    }
}

