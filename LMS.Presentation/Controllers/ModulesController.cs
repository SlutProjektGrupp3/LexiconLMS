using LMS.Shared.DTOs;
using LMS.Shared.DTOs.Modules;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Swashbuckle.AspNetCore.Annotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LMS.Presentation.Controllers
{
    [Route("api/modules")]
    [ApiController]
    [Authorize]
    public class ModulesController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public ModulesController(IServiceManager serviceManager)
        {
            this._serviceManager = serviceManager;
        }


        [HttpPost]
        [Authorize(Roles = "Teacher")]
        [SwaggerOperation(
            Summary = "Teacher can create a new module in a course",
            Description = "Creates a new module in a given course. Requires a valid JWT token with Teacher role.")]
        [SwaggerResponse(StatusCodes.Status201Created, "Module successfully created")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid input")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Course not found")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
        public async Task<IActionResult> CreateModule(CreateModuleDto createModuleDto)
        {
            try
            {
                var result = await _serviceManager.ModuleService.CreateModuleAsync(createModuleDto);

                if (!result.Succeeded)
                {
                    var error = result.Errors.FirstOrDefault();

                    // 404 → Course not found
                    if (error?.Code == "CourseNotFound")
                    {
                        return NotFound(new ProblemDetails
                        {
                            Title = "Course not found",
                            Status = StatusCodes.Status404NotFound,
                            Detail = error.Description
                        });
                    }

                    // 400 → Validation or other errors
                    return BadRequest(new ProblemDetails
                    {
                        Title = "Bad Request",
                        Status = StatusCodes.Status400BadRequest,
                        Detail = error?.Description ?? "Invalid request"
                    });
                }

                var module = result.CreatedModule!;

                // HATEOAS
                module = module with
                {
                    Links = new List<LinkDto>
                    {
                        new LinkDto
                        {
                            Href = Url.Action(nameof(GetModuleById), new { id = module.Id })!,
                            Rel = "self",
                            Method = "GET"
                        },
                        new LinkDto
                        {
                            Href = $"/api/courses/{createModuleDto.CourseId}",
                            Rel = "course",
                            Method = "GET"
                        }
                    }
                };

                // CreatedAtAction + Location Header
                return CreatedAtAction(nameof(GetModuleById), new { id = module.Id }, module);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "Internal Server Error",
                    Status = StatusCodes.Status500InternalServerError,
                    Detail = "An unexpected error occurred."
                });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Teacher")]
        [SwaggerOperation(
            Summary = "Teacher can delete a module in a course",
            Description = "Deletes a module in a given course. Requires a valid JWT token with Teacher role.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Module successfully deleted")]
        public async Task<IActionResult> DeleteModule(Guid id)
        {
            var result = await _serviceManager.ModuleService.DeleteModuleAsync(id);

            if (result.Succeeded)
            {
                return NoContent();
            }
            else
            {
                return result.Error.StatusCode switch
                {
                    ErrorStatusCode.NotFound => NotFound(result.Error),
                    ErrorStatusCode.BadRequest => BadRequest(result.Error),
                    ErrorStatusCode.Database => StatusCode(500, result.Error),
                    _ => StatusCode(500, result.Error)
                };
            }
        }

        [HttpPut("{moduleId}")]
        [Authorize(Roles = "Teacher")]
        [SwaggerOperation(
          Summary = "Teacher can update a module",
          Description = "Updates an existing module. Requires a valid JWT token with Teacher role.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Module successfully updated")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid module data")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Module not found")]
        public async Task<IActionResult> UpdateModule(Guid moduleId, [FromBody] UpdateModuleDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _serviceManager.ModuleService.UpdateModuleAsync(moduleId, dto);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpGet("{id:guid}", Name = "GetModuleById")]
        [Authorize]
        public async Task<IActionResult> GetModuleById(Guid id)
        {
            var module = await _serviceManager.ModuleService.GetModuleByIdAsync(id);

            if (module is null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Module not found",
                    Status = StatusCodes.Status404NotFound,
                    Detail = $"Module with id {id} was not found."
                });
            }

            // HATEOAS links
            module = module with
            {
                Links = new List<LinkDto>
                {
                    new LinkDto { Href = Url.Action(nameof(GetModuleById), new { id = module.Id })!, Rel = "self", Method = "GET" },
                    new LinkDto { Href = $"/api/courses/{module.CourseId}", Rel = "course", Method = "GET" }
                }
            };
            
            return Ok(module);
        }
    }
}
