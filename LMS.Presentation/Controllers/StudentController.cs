using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using System.Security.Claims;

namespace LMS.Presentation.Controllers;

[ApiController]
[Route("api/student")]
[Authorize(Roles = "Student")]
public class StudentController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public StudentController(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }

    [HttpGet("my-course")]
    public async Task<IActionResult> GetMyCourse()
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userIdValue))
                return Unauthorized(new ProblemDetails
                {
                    Title = "Unauthorized",
                    Status = StatusCodes.Status401Unauthorized,
                    Detail = "User identifier not found in token."
                });

        var userId = Guid.Parse(userIdValue);

        var course = await _serviceManager.StudentService.GetMyCourseAsync(userId);

            if (course is null)
                return NotFound(new ProblemDetails
                {
                    Title = "Course not found",
                    Status = StatusCodes.Status404NotFound,
                    Detail = "You are not enrolled in any course."
                });

        return Ok(course);
    }
}
