using LMS.Shared.DTOs.CourseDtos;
using LMS.Shared.Request;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace LMS.Presentation.Controllers
{
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
        public async Task<IActionResult> GetCourses()
        {
            var courses = await serviceManager.CourseService.GetAllCoursesAsync();
            //var dto = _mapper.Map<List<CourseDto>>(courses);
            return Ok(courses);
        }

        // GET: api/courses/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetCourse(Guid id)
        {
            var courses = await serviceManager.CourseService.GetCourseAsync(id, trackChanges: false);
            return Ok(courses);
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
        public async Task<ActionResult<CourseDto>> PostCourse(CourseCreateDto dto)
        {
            var createdDto = await serviceManager.CourseService.CreateCourseAsync(dto);

            return CreatedAtAction(nameof(GetCourse), new { id = createdDto.Id }, createdDto);
        }
    }
}
