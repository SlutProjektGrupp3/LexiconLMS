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
        var activities = await _serviceManager.ActivityService
            .GetActivitiesForModuleAsync(moduleId);
        return Ok(activities);
    }

    [HttpPost]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> CreateActivity([FromBody] CreateActivityDto dto)
    {
        var result = await _serviceManager.ActivityService
            .CreateActivityAsync(dto);
        
        return CreatedAtAction(
                nameof(GetActivitiesForModule),
                new { moduleId = result.ModuleId },
                 result
            );        
    }

    [HttpGet("types")]
    public async Task<IActionResult> GetActivityTypes()
    {
        var types = await _serviceManager.ActivityService
            .GetAllActivityTypesAsync();

        return Ok(types);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> DeleteActivity(Guid id)
    {
        await _serviceManager.ActivityService.DeleteActivityAsync(id);
        
        return NoContent();       
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> UpdateActivity(Guid id, UpdateActivityDto dto)
    {
        var result = await _serviceManager.ActivityService
            .UpdateActivityAsync(id, dto);            

            return Ok(result);        
    }
}
