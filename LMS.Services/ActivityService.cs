using Domain.Contracts.Repositories;
using LMS.Shared.DTOs;
using Service.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Services
{
    public class ActivityService : IActivityService
    {
        private readonly IActivityRepository _repository;

        public ActivityService(IActivityRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ActivityDto>> GetActivitiesForModuleAsync(Guid moduleId)
        {
            var activitiesFromDb = await _repository.GetActivitiesByModuleIdAsync(moduleId, trackChanges: false);

            var dtoList = activitiesFromDb.Select(a => new ActivityDto(
                a.Id,
                a.Name,
                a.Type.Name,
                a.Description,
                a.StartDate,
                a.EndDate
            )).ToList();

            return dtoList;
        }
    }
}
