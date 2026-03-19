
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Courses;
using LMS.Shared.DTOs;

namespace LMS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;

    public CoursesController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    [HttpPost]
    [Authorize(Roles = "Teacher")]

    public async Task<IActionResult> CreateCourse([FromBody] CreateCourseDto dto)
    {
        try
        {
            var createdCourse = await _courseService.CreateCourseAsync(dto);
            return Ok(createdCourse);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
