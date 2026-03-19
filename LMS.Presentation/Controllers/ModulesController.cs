using LMS.Shared.DTOs.Modules;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Swashbuckle.AspNetCore.Annotations;

namespace LMS.Presentation.Controllers
{
    [Route("api/modules/create")]
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
            var result = await serviceManager.ModulesService.CreateModuleAsync(createModuleDto);

            return result.Succeeded ? StatusCode(StatusCodes.Status201Created) : BadRequest(result.Errors); 
        }
    }
}
