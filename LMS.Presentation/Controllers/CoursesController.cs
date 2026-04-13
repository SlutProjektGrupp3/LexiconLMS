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
    [HttpGet("active")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> GetActiveCourses()
    {
        var courses = await _serviceManager.CourseService.GetActiveCoursesAsync();
        return Ok(courses);
    }

    // GET: api/courses/{id}
    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetCourseById(Guid id)
    {
        var courseDetails = await _serviceManager.CourseService.GetCourseByIdAsync(id);
        
        return Ok(courseDetails);
    }

    // POST: api/courses
    [HttpPost]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> CreateCourse([FromBody] CreateCourseDto dto)
    {
        var createdCourse = await _serviceManager.CourseService.CreateCourseAsync(dto);
        
        return CreatedAtAction(nameof(GetCourseById),new { id = createdCourse.Id },createdCourse);                
    }

    // PUT: api/courses/{id} 
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> UpdateCourse(Guid id, [FromBody] UpdateCourseDto updateCourseDto)
    {
        await _serviceManager.CourseService.UpdateCourseAsync(id, updateCourseDto, trackChanges: true);
        
        return NoContent();        
    }

    // DELETE: api/courses/{id}
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> DeleteCourse(Guid id)
    {
        await _serviceManager.CourseService.DeleteCourseAsync(id, trackChanges: true);
        
        return NoContent();        
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

        // Return CourseDetailsDto items directly in the paged response
        var dto = new
        {
            Items = items,
            Total = total
        };

        return Ok(dto);
    }

    // GET: api/courses/{courseId}/participants
    [HttpGet("{courseId}/participants")]
    public async Task<IActionResult> GetParticipants(Guid courseId)
    {
        var participants = await _serviceManager.CourseService
            .GetParticipantsAsync(courseId);

        return Ok(participants);
    }
}