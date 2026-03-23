using Microsoft.AspNetCore.Mvc;
using Service.Contracts;


namespace LMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CoursesController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public CoursesController(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCourse(Guid id)
    {
        var course = await _serviceManager.CourseService.GetCourseByIdAsync(id);

        if (course == null)
            return NotFound();

        return Ok(course);
    }
}
