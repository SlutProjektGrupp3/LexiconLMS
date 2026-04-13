using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using Domain.Models.Exceptions;
using LMS.Shared.DTOs;
using LMS.Shared.DTOs.Course;
using LMS.Shared.DTOs.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Service.Contracts;
using System.Data;
using System.Reflection;

namespace LMS.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;

    public UserService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
    }

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        var users = await _unitOfWork.UserRepository.GetAllUsersAsync();
        
        var dtoList = new List<UserDto>();

        foreach (var user in users)
        {
            var role = await _userManager.GetRolesAsync(user);
            var course = await _unitOfWork.CourseRepository.GetCourseByUserIdAsync(user.Id);

            dtoList.Add(new UserDto
            (
                Id: user.Id,
                FirstName: user.FirstName,
                LastName: user.LastName,
                Email: user.Email,
                RoleName: role.FirstOrDefault(),
                Course: course != null ? new CourseDto(course.Id, course.Name, course.Description, course.StartDate, course.EndDate) : null
            ));
        }

        return dtoList;
    }

    public async Task<UserDto?> GetUserByIdAsync(string id)
    {
        var user = await _unitOfWork.UserRepository.GetUserByIdAsync(id);
        if (user == null)
            return null;
        var role = await _userManager.GetRolesAsync(user);
        return new UserDto
        (
            Id: user.Id,
            FirstName: user.FirstName,
            LastName: user.LastName,
            Email: user.Email!,
            RoleName: role.FirstOrDefault(),
            Course: user.Course == null ? null : new CourseDto(user.Course.Id, user.Course.Name, user.Course.Description, user.Course.StartDate, user.Course.EndDate)
        );
    }

    public async Task<ResultDto<UserDto>> CreateUserAsync(CreateUserDto userCreateDto)
    {
        var user = new ApplicationUser
        {
            FirstName = userCreateDto.FirstName,
            LastName = userCreateDto.LastName,
            Email = userCreateDto.Email,
            UserName = userCreateDto.Email,
            CourseId = userCreateDto.CourseId
        };

        try
        {
            var createResult = await _userManager.CreateAsync(user, _configuration["password"]);

            if (!createResult.Succeeded)
            {
                var errors = createResult.Errors
                    .Select(e => new ErrorDto
                    {
                        Code = e.Code,
                        Description = e.Description
                    })
                    .ToList();

                return ResultDto<UserDto>.Failed(errors);
            }

            var roleResult = await _userManager.AddToRoleAsync(user, userCreateDto.RoleName);

            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);

                var errors = roleResult.Errors
                    .Select(e => new ErrorDto
                    {
                        Code = e.Code,
                        Description = e.Description
                    })
                    .ToList();

                return ResultDto<UserDto>.Failed(errors);
            }

            var createdUser = await _userManager.Users
                .Where(u => u.Id == user.Id)
                .Select(u => new UserDto
                (
                    Id: u.Id,
                    FirstName: u.FirstName,
                    LastName: u.LastName,
                    Email: u.Email!,
                    RoleName: userCreateDto.RoleName,
                    Course: u.Course == null ? null : new CourseDto(u.Course.Id, u.Course.Name, u.Course.Description, u.Course.StartDate, u.Course.EndDate)
                )).FirstOrDefaultAsync();

            return ResultDto<UserDto>.Success(createdUser);
        }
        catch
        {
            var errors = new List<ErrorDto>
        {
            new ErrorDto
            {
                Code = "USER_ERROR:DB",
                Description = "An error occurred while saving the user to the database."
            }
        };

            return ResultDto<UserDto>.Failed(errors);
        }
    }

    public async Task<ResultDto<UserDto>> UpdateUserAsync(string id, UpdateUserDto userUpdateDto)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return ResultDto<UserDto>.Failed(new List<ErrorDto> { new ErrorDto { Code = "USER_NOT_FOUND", Description = "User not found." } });

        user.FirstName = userUpdateDto.FirstName;
        user.LastName = userUpdateDto.LastName;
        user.Email = userUpdateDto.Email;
        user.UserName = userUpdateDto.Email;
        user.CourseId = userUpdateDto.CourseId;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = result.Errors
                .Select(e => new ErrorDto
                {
                    Code = e.Code,
                    Description = e.Description
                })
                .ToList();

            return ResultDto<UserDto>.Failed(errors);
        }

        var currentRoles = await _userManager.GetRolesAsync(user);
        var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
        if (!removeResult.Succeeded)
        {
            var errors = removeResult.Errors
                .Select(e => new ErrorDto
                {
                    Code = e.Code,
                    Description = e.Description
                })
                .ToList();

            return ResultDto<UserDto>.Failed(errors);
        }

        var addRoleResult = await _userManager.AddToRoleAsync(user, userUpdateDto.RoleName);
        if (!addRoleResult.Succeeded)
        {
            var errors = addRoleResult.Errors
                .Select(e => new ErrorDto
                {
                    Code = e.Code,
                    Description = e.Description
                })
                .ToList();

            return ResultDto<UserDto>.Failed(errors);
        }

        await _unitOfWork.CompleteAsync();

        var updatedUser = await _userManager.Users
            .Where(u => u.Id == user.Id)
            .Select(u => new UserDto
            (
                Id: u.Id,
                FirstName: u.FirstName,
                LastName: u.LastName,
                Email: u.Email!,
                RoleName: userUpdateDto.RoleName,
                Course: u.Course == null ? null : new CourseDto(u.Course.Id, u.Course.Name, u.Course.Description, u.Course.StartDate, u.Course.EndDate)
            )).FirstOrDefaultAsync();

        return ResultDto<UserDto>.Success(updatedUser);
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
            "Teacher",
            null
        ));
    }
}

