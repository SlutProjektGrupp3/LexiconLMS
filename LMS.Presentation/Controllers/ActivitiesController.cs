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
        private readonly IServiceManager _serviceManager;

        public ActivitiesController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetActivitiesForModule(Guid moduleId)
        {
            var activities = await _serviceManager.ActivityService.GetActivitiesForModuleAsync(moduleId);
            return Ok(activities);
        }
    }
}
