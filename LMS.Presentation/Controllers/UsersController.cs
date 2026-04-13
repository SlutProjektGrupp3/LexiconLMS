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
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto createUserDto)
    {
        var result = await _serviceManager.UserService.CreateUserAsync(createUserDto);
        return result.Succeeded ? CreatedAtAction(nameof(GetAllUsers), new { id = result.Data!.Id }, result) : BadRequest(result.Errors);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserDto updateUserDto)
    {
        var result = await _serviceManager.UserService.UpdateUserAsync(id, updateUserDto);
        return result.Succeeded ? Ok(result) : BadRequest(result.Errors);
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

}
