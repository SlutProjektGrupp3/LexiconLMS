using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Http.HttpResults;
using LMS.Shared.DTOs.Shared;

namespace LMS.Presentation.Controllers
{
    [ApiController]
    [Route("api")]
    public class ApiController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public ApiController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "API Root",
            Description = "Retruns api endpoints that can be called next")]
        [SwaggerResponse(StatusCodes.Status200OK, "Endpoint list")]
        public async Task<IActionResult> GetApiRoot()
        {
            List<LinkDto> apiEndpoints = new List<LinkDto>
            {
                new LinkDto { Rel = "GetCourses", Href = Url.Action("GetCourses", "Courses") ?? "#", Method = "GET" },
            };

            return Ok(apiEndpoints);
        }
    }
}
