using LMS.Shared.DTOs.AuthDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Swashbuckle.AspNetCore.Annotations;

namespace LMS.Presentation.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public AuthController(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "Register a new user",
        Description = "Creates a new user account with the provided registration details."
    )]
    [SwaggerResponse(StatusCodes.Status201Created, "User successfully registered")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid input or registration failed")]
    public async Task<IActionResult> RegisterUser(UserRegistrationDto userRegistrationDto)
    {
        IdentityResult result = await _serviceManager.AuthService.RegisterUserAsync(userRegistrationDto);
        return result.Succeeded ? StatusCode(StatusCodes.Status201Created) : BadRequest(result.Errors);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [SwaggerOperation(
        Summary = "Authenticate user",
        Description = "Validates user credentials and returns a JWT token for authorization."
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Authentication successful", typeof(TokenDto))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Invalid username or password")]
    public async Task<IActionResult> Authenticate(UserAuthDto user)
    {
        if (!await _serviceManager.AuthService.ValidateUserAsync(user))
            return Unauthorized();

        var tokenDto = await _serviceManager.AuthService.CreateTokenAsync(addTime: true);
        return Ok(tokenDto);
    }
}
