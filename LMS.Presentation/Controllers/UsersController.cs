using LMS.Shared.DTOs.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;

namespace LMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]

public class UsersController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public UsersController(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }

    [Authorize(Roles = "Teacher")]
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _serviceManager.UserService.GetAllUsersAsync();

        return Ok(users ?? new List<UserDto>());
    }

    [Authorize(Roles = "Teacher")]
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto userCreateDto)
    {
        var result = await _serviceManager.UserService.CreateUserAsync(userCreateDto);
        return result.Succeeded ? CreatedAtAction(nameof(GetAllUsers), new { id = result.CreatedUser!.Id }, result) : BadRequest(result.Errors);
    }

    [Authorize(Roles = "Teacher")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        await _serviceManager.UserService.DeleteUserAsync(id);
        return NoContent();
    }

    [HttpGet]
    [Route("roles")]
    public async Task<IActionResult> GetAllRoles()
    {
        var roles = await _serviceManager.UserService.GetAllRolesAsync();
        if(roles == null || !roles.Any())
        {
            return NotFound("No roles found.");
        }
        return Ok(roles);
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserDto dto)
    {
        var updatedUser = await _serviceManager.UserService.UpdateUserAsync(id, dto);
        return Ok(updatedUser);
    }

    [HttpGet]
    [Route("count-by-role/{roleName}")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> GetUsersCountByRole(string roleName)
    {
        if (string.IsNullOrWhiteSpace(roleName))
            return BadRequest("Role name is required.");

        var count = await _serviceManager.UserService.GetUsersCountByRoleAsync(roleName);
        return Ok(count);
    }
    
    [HttpGet("teachers")]
    [Authorize(Roles = "Teacher,Student")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetTeachers()
    {
        var teachers = await _serviceManager.UserService.GetTeachersAsync();
        return Ok(teachers);
    }
}
