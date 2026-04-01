using LMS.Shared.DTOs;
using LMS.Shared.DTOs.CourseDtos;
using Microsoft.AspNetCore.Authorization;
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

    [HttpGet]
    [Authorize(Roles = "Teacher")]
    // GET: api/courses
    public async Task<IActionResult> GetCourses()
    {
        var courses = await serviceManager.CourseService.GetAllCoursesAsync();
        return Ok(courses);
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetCourseById(Guid id)
    {
        var courseDetails = await serviceManager.CourseService.GetCourseByIdAsync(id);
        if (courseDetails == null)
            return NotFound();
        return Ok(courseDetails);
    }

    [HttpPost]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> CreateCourse([FromBody] CreateCourseDto dto)
    {
        var createdCourse = await _courseService.CreateCourseAsync(dto);
        return Ok(createdCourse);
    }


    [Authorize(Roles = "Teacher")]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateCourse(Guid id, [FromBody] UpdateCourseDto updateCourseDto)
        {
            if (updateCourseDto is null)
                return BadRequest("UpdateCourseDto is null.");

            await serviceManager.CourseService.UpdateCourseAsync(id, updateCourseDto, trackChanges: true);

            return NoContent();
        }

        [Authorize(Roles = "Teacher")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCourse(Guid id)
    {
        await serviceManager.CourseService.DeleteCourseAsync(id, trackChanges: true);
        return NoContent();
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
}
