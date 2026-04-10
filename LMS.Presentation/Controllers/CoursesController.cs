using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using LMS.Shared.DTOs.Course;

namespace LMS.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]

public class CoursesController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public CoursesController(IServiceManager serviceManager)
 
    {
        _serviceManager = serviceManager;
    }

    // GET: api/courses
    [HttpGet]
    [Authorize(Roles = "Teacher")]    
    public async Task<IActionResult> GetCourses()
    {
        var courses = await _serviceManager.CourseService.GetAllCoursesAsync();
        return Ok(courses);
    }

    // GET: api/courses/{id}
    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetCourseById(Guid id)
    {
        var courseDetails = await _serviceManager.CourseService.GetCourseByIdAsync(id);
        if (courseDetails is null)
        {
                return NotFound(new ProblemDetails
                {
                    Title = "Course not found",
                    Status = StatusCodes.Status404NotFound,
                    Detail = $"Course with id {id} was not found."
                });
        }

        return Ok(courseDetails);
    }

    // POST: api/courses
    [HttpPost]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> CreateCourse([FromBody] CreateCourseDto dto)
    {
        var createdCourse = await _serviceManager.CourseService.CreateCourseAsync(dto);
        
        if (createdCourse.Succeeded)
            return CreatedAtAction(nameof(GetCourseById),new { id = createdCourse.Data!.Id },createdCourse);
        
        var error = createdCourse.Errors.First();

        return error.Code switch
        {
            "CourseNotFound" => NotFound(new ProblemDetails
            {
                Title = "Course not found",
                Status = StatusCodes.Status404NotFound,
                Detail = error.Description
            }),
            _ => BadRequest(new ProblemDetails
            {
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = error.Description
            })
        };
    }

    // PUT: api/courses/{id} 
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> UpdateCourse(Guid id, [FromBody] UpdateCourseDto updateCourseDto)
    {
        if (updateCourseDto is null)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "UpdateCourseDto is null"
            });
        }

        try
        {
            await _serviceManager.CourseService.UpdateCourseAsync(id, updateCourseDto, trackChanges: true);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Course not found",
                Status = StatusCodes.Status404NotFound,
                Detail = $"Course with id {id} was not found."
            });
        }
    }

    // DELETE: api/courses/{id}
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> DeleteCourse(Guid id)
    {
        try
        {
            var result = await _serviceManager.CourseService.DeleteCourseAsync(id, trackChanges: true);
            
            if (!result.Succeeded)
            {
                var error = result.Errors.FirstOrDefault();
                if (error == null)
                {
                    return StatusCode(500, "Unknown error");
                }
                if (error.Code == "CourseNotFound")
                {
                    return NotFound(new ProblemDetails
                    {
                        Title = "Course not found",
                        Status = StatusCodes.Status404NotFound,
                        Detail = error.Description
                    });
                }

                return StatusCode(500, new ProblemDetails
                {
                    Title = "Internal Server Error",
                    Status = StatusCodes.Status500InternalServerError,
                    Detail = error.Description
                });
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ProblemDetails
            {
                Title = "Internal Server Error",
                Status = StatusCodes.Status500InternalServerError,
                Detail = ex.Message
            });
        }
    }

    [Authorize(Roles = "Teacher")]
    [HttpPost("{courseId}/students/{studentId}")]
    public async Task<IActionResult> AddStudentToCourse(Guid courseId, string studentId)
    {
        await _serviceManager.CourseService.AddStudentToCourseAsync(courseId, studentId);
        return NoContent();
    }

    [Authorize(Roles = "Teacher")]
    [HttpGet("{courseId}/available-students")]
    public async Task<IActionResult> GetAvailableStudents()
    {
        var students = await _serviceManager.CourseService.GetAvailableStudentsAsync();
        return Ok(students);
    }

    // GET: api/courses/summary
    [HttpGet("summary")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> GetCourseSummaries([FromQuery] string? search, [FromQuery] bool? active, [FromQuery] int page = 1, [FromQuery] int pageSize = 12)
    {
        var (items, total) = await _serviceManager.CourseService.GetCourseSummariesAsync(search, active, page, pageSize);

        var dto = new CourseSummaryPagedDto
        {
            Items = items.ToList(),
            Total = total
        };

        return Ok(dto);
    }
}