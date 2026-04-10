using LMS.Shared.DTOs.User;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;

namespace LMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public UsersController(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized("User ID not found");

        var user = await _serviceManager.UserService.GetUserByIdAsync(userId);
        if (user == null)
            return NotFound($"User with ID {userId} not found.");

        return Ok(user);
    }

    [HttpGet]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _serviceManager.UserService.GetAllUsersAsync();

        return Ok(users ?? new List<UserDto>());
    }

    [HttpGet]
    [Route("{id}")]
    [Authorize]
    public async Task<IActionResult> GetUserById(string id)
    {
        var user = await _serviceManager.UserService.GetUserByIdAsync(id);
        if (user == null)
            return NotFound($"User with ID {id} not found.");
        return Ok(user);
    }


    [HttpPost]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto userCreateDto)
    {
        var result = await _serviceManager.UserService.CreateUserAsync(userCreateDto);
        return result.Succeeded ? CreatedAtAction(nameof(GetAllUsers), new { id = result.CreatedUser!.Id }, result) : BadRequest(result.Errors);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Teacher")]
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
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> GetAllRoles()
    {
        var roles = await _serviceManager.UserService.GetAllRolesAsync();
        if(roles == null || !roles.Any())
        {
            return NotFound("No roles found.");
        }
        return Ok(roles);
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
