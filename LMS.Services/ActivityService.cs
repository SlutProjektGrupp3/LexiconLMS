using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Shared.DTOs.Activity;
using Service.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Services
{
    public class ActivityService : IActivityService
    {
        private readonly IActivityRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public ActivityService(IActivityRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
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
                a.EndDate,
                a.TypeId
            )).ToList();

            return dtoList;
        }

        public async Task<ActivityDto> CreateActivityAsync(CreateActivityDto dto)
        {
            var newActivity = new ModuleActivity
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                ModuleId = dto.ModuleId,
                TypeId = dto.TypeId.Value,
            };

            _repository.Create(newActivity);
            await _unitOfWork.CompleteAsync();
            var activityType = await _repository.GetActivityTypeByIdAsync(newActivity.ActivityTypeId);

            return new ActivityDto(
                newActivity.Id,
                newActivity.Name,
                activityType?.Name ?? "Unknown",
                newActivity.Description,
                newActivity.StartDate,
                newActivity.EndDate,
                newActivity.TypeId
            );
        }

        public async Task<IEnumerable<ActivityTypeDto>> GetAllActivityTypesAsync()
        {
            var typesFromDb = await _repository.GetAllActivityTypesAsync(trackChanges: false);

            return typesFromDb.Select(t => new ActivityTypeDto
            {
                Id = t.Id,
                Name = t.Name
            }).ToList();
        }

        public async Task DeleteActivityAsync(Guid activityId)
        {
            var activity = await _repository.GetActivityByIdAsync(activityId, trackChanges: false);
            if (activity != null)
            {
                _repository.Delete(activity); 
                await _unitOfWork.CompleteAsync();
            }
        }
        public async Task<ActivityDto> UpdateActivityAsync(Guid activityId, UpdateActivityDto dto)
        {
            var activity = await _repository.GetActivityByIdAsync(activityId, trackChanges: true);

            if (activity == null)
            {
                throw new Exception($"Activity could not be found.");
            }

            activity.Name = dto.Name;
            activity.Description = dto.Description;
            activity.StartDate = dto.StartDate;
            activity.EndDate = dto.EndDate;
            activity.TypeId = dto.TypeId;

            await _unitOfWork.CompleteAsync();

            var activityType = await _repository.GetActivityTypeByIdAsync(activity.TypeId);

            return new ActivityDto(
                activity.Id,
                activity.Name,
                activityType?.Name ?? "Unknown",
                activity.Description,
                activity.StartDate,
                activity.EndDate,
                activity.TypeId
            );
        }
    }

}