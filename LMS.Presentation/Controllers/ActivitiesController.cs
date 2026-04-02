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
    public async Task<IActionResult> CreateActivity([FromBody] CreateActivityDto dto)
    {
        //var createdActivity = await _serviceManager.ActivityService.CreateActivityAsync(dto);
        //return StatusCode(StatusCodes.Status201Created, createdActivity);
        try
        {
            var result = await _serviceManager.ActivityService.CreateActivityAsync(dto);

            if (!result.Succeeded)
            {
                var error = result.Errors.FirstOrDefault();

                return BadRequest(new ProblemDetails
                {
                    Title = "Bad Request",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = error?.Description ?? "Invalid request"
                });
            }

            var activity = result.Data!;

            return CreatedAtAction(
                nameof(GetActivitiesForModule),
                new { moduleId = activity.ModuleId },
                 activity
            );
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Internal Server Error",
                Status = StatusCodes.Status500InternalServerError,
                Detail = "An unexpected error occurred."
            });
        }
    }

    [HttpGet("types")]
    public async Task<IActionResult> GetActivityTypes()
    {
        var types = await _serviceManager.ActivityService.GetAllActivityTypesAsync();
        return Ok(types);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> DeleteActivity(Guid id)
    {
        //await _serviceManager.ActivityService.DeleteActivityAsync(id);
        //return NoContent();
        try
        {
            var result = await _serviceManager.ActivityService.DeleteActivityAsync(id);

            if (!result.Succeeded)
            {
                var error = result.Errors.FirstOrDefault();

                if (error?.Code == "ActivityNotFound")
                {
                    return NotFound(new ProblemDetails
                    {
                        Title = "Activity not found",
                        Status = StatusCodes.Status404NotFound,
                        Detail = error.Description
                    });
                }

                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "Internal Server Error",
                    Status = StatusCodes.Status500InternalServerError,
                    Detail = error?.Description
                });
            }

            return NoContent();
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Internal Server Error",
                Status = StatusCodes.Status500InternalServerError,
                Detail = "An unexpected error occurred."
            });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> UpdateActivity(Guid id, UpdateActivityDto dto)
    {
        //if (dto.EndDate < dto.StartDate)
        //{
        //    return BadRequest("Start date before end date.");
        //}
        //var updatedActivity = await _serviceManager.ActivityService.UpdateActivityAsync(id, dto);
        //return Ok(updatedActivity);
        if (dto.EndDate < dto.StartDate)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid Dates",
                Status = StatusCodes.Status400BadRequest,
                Detail = "End date must be after start date."
            });
        }

        try
        {
            var result = await _serviceManager.ActivityService.UpdateActivityAsync(id, dto);

            if (!result.Succeeded)
            {
                var error = result.Errors.FirstOrDefault();

                return error?.Code switch
                {
                    "ActivityNotFound" => NotFound(new ProblemDetails
                    {
                        Title = "Activity not found",
                        Status = StatusCodes.Status404NotFound,
                        Detail = error.Description
                    }),

                    _ => BadRequest(new ProblemDetails
                    {
                        Title = "Bad Request",
                        Status = StatusCodes.Status400BadRequest,
                        Detail = error?.Description
                    })
                };
            }

            return Ok(result.Data);
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Internal Server Error",
                Status = StatusCodes.Status500InternalServerError,
                Detail = "An unexpected error occurred."
            });
        }
    }
}
