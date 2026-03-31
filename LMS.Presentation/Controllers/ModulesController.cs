using LMS.Shared.DTOs.Modules;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Swashbuckle.AspNetCore.Annotations;

namespace LMS.Presentation.Controllers
{
    [Route("api/modules")]
    [ApiController]
    [Authorize]
    public class ModulesController : ControllerBase
    {
        private readonly IServiceManager serviceManager;

        public ModulesController(IServiceManager serviceManager)
        {
            this.serviceManager = serviceManager;
        }


        [HttpPost]
        [Authorize(Roles = "Teacher")]
        [SwaggerOperation(
            Summary = "Teacher can create a new module in a course",
            Description = "Creates a new module in a given course. Requires a valid JWT token with Teacher role.")]
        [SwaggerResponse(StatusCodes.Status201Created, "Module successfully created")]
        public async Task<IActionResult> CreateModule(CreateModuleDto createModuleDto)
        {
            var result = await serviceManager.ModuleService.CreateModuleAsync(createModuleDto);

            return result.Succeeded ? StatusCode(StatusCodes.Status201Created, result) : BadRequest(result.Errors); 
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
                await serviceManager.ModuleService.UpdateModuleAsync(moduleId, dto);
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
    }
}
