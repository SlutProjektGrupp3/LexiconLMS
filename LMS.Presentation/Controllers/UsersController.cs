using LMS.Shared.DTOs.User;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
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
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var user = await _serviceManager.UserService.GetUserByIdAsync(userId);        

        return Ok(user);
    }

    [HttpGet]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _serviceManager.UserService.GetAllUsersAsync();

        return Ok(users);
    }

    [HttpGet]
    [Route("{id}")]
    [Authorize]
    public async Task<IActionResult> GetUserById(string id)
    {
        var user = await _serviceManager.UserService.GetUserByIdAsync(id);
        
        return Ok(user);
    }


    [HttpPost]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto createUserDto)
    {
        var result = await _serviceManager.UserService.CreateUserAsync(createUserDto);

        return CreatedAtAction(
            nameof(GetUserById),
            new { id = result.Id },
            result
        );
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserDto updateUserDto)
    {
        var result = await _serviceManager.UserService.UpdateUserAsync(id, updateUserDto);
        return Ok(result);
    }

    [Authorize(Roles = "Teacher")]
    [HttpDelete("{id}")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        await _serviceManager.UserService.DeleteUserAsync(id);
        return NoContent();
    }

    [HttpGet]
    [Route("roles")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> GetAllRoles()
    {
        var roles = await _serviceManager.UserService.GetAllRolesAsync();        
        return Ok(roles);
    }

    [HttpGet]
    [Route("count-by-role/{roleName}")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> GetUsersCountByRole(string roleName)
    {
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
