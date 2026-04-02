using LMS.Shared.DTOs;
using LMS.Shared.DTOs.CourseDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;

namespace LMS.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]

public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;
    private readonly IServiceManager serviceManager;


    public CoursesController(IServiceManager serviceManager, ICourseService courseService)
 
    {
        _courseService = courseService;
        this.serviceManager = serviceManager;
    }

    // GET: api/courses
    [HttpGet]
    [Authorize(Roles = "Teacher")]    
    public async Task<IActionResult> GetCourses()
    {
        var courses = await serviceManager.CourseService.GetAllCoursesAsync();
        return Ok(courses);
    }

    // GET: api/courses/{id}
    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetCourseById(Guid id)
    {
        var courseDetails = await serviceManager.CourseService.GetCourseByIdAsync(id);
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
        var createdCourse = await _courseService.CreateCourseAsync(dto);
        
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
            await serviceManager.CourseService.UpdateCourseAsync(id, updateCourseDto, trackChanges: true);
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
            var result = await serviceManager.CourseService.DeleteCourseAsync(id, trackChanges: true);
            
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
        await _courseService.AddStudentToCourseAsync(courseId, studentId);
        return NoContent();
    }

    [Authorize(Roles = "Teacher")]
    [HttpGet("{courseId}/available-students")]
    public async Task<IActionResult> GetAvailableStudents()
    {
        var students = await _courseService.GetAvailableStudentsAsync();
        return Ok(students);
    }

    [HttpGet("{courseId}/participants")]
    public async Task<IActionResult> GetParticipants(Guid courseId)
    {
        var participants = await serviceManager.CourseService
            .GetParticipantsAsync(courseId);

        return Ok(participants);
    }
}