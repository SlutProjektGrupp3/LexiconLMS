using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;

namespace LMS.Presentation.Controllers;

[Route("api/activities")]
[ApiController]
[Authorize] 
public class ActivitiesController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public ActivitiesController(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }

    [HttpGet("modules/{moduleId}")]
    public async Task<IActionResult> GetActivitiesForModule(Guid moduleId)
    {
        var activities = await _serviceManager.ActivityService.GetActivitiesForModuleAsync(moduleId);
        return Ok(activities);
    }
}
