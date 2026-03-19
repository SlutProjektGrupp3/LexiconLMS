using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Presentation.Controllers
{
    [Route("api/modules/{moduleId}/activities")]
    [ApiController]
    [Authorize] 
    public class ActivitiesController : ControllerBase
    {
        private readonly IActivityService _activityService;

        public ActivitiesController(IActivityService activityService)
        {
            _activityService = activityService;
        }

        [HttpGet]
        public async Task<IActionResult> GetActivitiesForModule(Guid moduleId)
        {
            var activities = await _activityService.GetActivitiesForModuleAsync(moduleId);
            return Ok(activities);
        }
    }
}
