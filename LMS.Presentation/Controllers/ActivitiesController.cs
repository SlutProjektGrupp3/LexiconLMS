using LMS.Shared.DTOs.Activity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

    [HttpPost]
    public async Task<IActionResult> CreateActivity(CreateActivityDto dto)
    {
        var createdActivity = await _serviceManager.ActivityService.CreateActivityAsync(dto);
        return StatusCode(StatusCodes.Status201Created, createdActivity);
    }
}
