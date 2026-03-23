using LMS.Shared.DTOs.CourseDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;

namespace LMS.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]

public class CoursesController : ControllerBase
{
    private readonly IServiceManager serviceManager;
    public CoursesController(IServiceManager serviceManager)
    {
        this.serviceManager = serviceManager;
    }

    [HttpGet]
    [Authorize(Roles = "Teacher")]
    // GET: api/courses
    public async Task<IActionResult> GetCourses()
    {
        var courses = await serviceManager.CourseService.GetAllCoursesAsync();
        //var dto = _mapper.Map<List<CourseDto>>(courses);
        return Ok(courses);
    }

    // GET: api/courses/{id}
    //[HttpGet("{id:guid}")]
    //public async Task<IActionResult> GetCourse(Guid id)
    //{
    //    var courses = await serviceManager.CourseService.GetCourseAsync(id, trackChanges: false);
    //    if (courses == null)
    //        return NotFound();
    //    return Ok(courses);
    //}

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetCourseById(Guid id)
    {
        var courseDetails = await serviceManager.CourseService.GetCourseByIdAsync(id);
        if (courseDetails == null)
            return NotFound();
        return Ok(courseDetails);
    }

    //// GET: api/courses?PageNumber=1&PageSize=10
    //[HttpGet]
    //public async Task<ActionResult<IEnumerable<CourseDto>>> GetCourse([FromQuery] CourseRequestParams requestParams)
    //{
    //    var pagedResult = await serviceManager.CourseService.GetCoursesAsync(requestParams);

    //    Response.Headers.Append(new("X-Pagination", JsonSerializer.Serialize(pagedResult.metaData)));

    //    return Ok(pagedResult.courseDtos);
    //}

    [HttpPost]
    [Authorize(Roles = "Teacher")]
    public async Task<ActionResult<CourseDto>> PostCourse(CourseCreateDto dto)
    {
        var createdDto = await serviceManager.CourseService.CreateCourseAsync(dto);

        return CreatedAtAction(nameof(GetCourseById), new { id = createdDto.Id }, createdDto);
    }
}
