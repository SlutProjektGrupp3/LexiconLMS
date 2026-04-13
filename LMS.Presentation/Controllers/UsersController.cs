using LMS.Shared.DTOs.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;

namespace LMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Teacher")]
public class UsersController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public UsersController(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _serviceManager.UserService.GetAllUsersAsync();

        return Ok(users ?? new List<UserDto>());
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto userCreateDto)
    {
        var result = await _serviceManager.UserService.CreateUserAsync(userCreateDto);
        return result.Succeeded ? CreatedAtAction(nameof(GetAllUsers), new { id = result.CreatedUser!.Id }, result) : BadRequest(result.Errors);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        try
        {
            await _serviceManager.UserService.DeleteUserAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex) 
        {
            return BadRequest(ex.Message);
        }

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
}
