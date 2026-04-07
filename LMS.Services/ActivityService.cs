using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using Domain.Models.Exceptions;
using LMS.Shared.DTOs;
using LMS.Shared.DTOs.Activity;
using Service.Contracts;
using System;
using System.Collections.Generic;
using System.Reflection;
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
                a.TypeId,
                a.ModuleId
            )).ToList();

            return dtoList;
        }

        public async Task<ResultDto<ActivityDto>> CreateActivityAsync(CreateActivityDto dto)
        {
            var moduleExists = await _repository.ModuleExistsAsync(dto.ModuleId);
            if (!moduleExists)
                return ResultDto<ActivityDto>.Failed(new ErrorDto
                {
                    Code = "ModuleNotFound",
                    Description = $"Module with id {dto.ModuleId} was not found."
                });

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

            var activityType = await _repository.GetActivityTypeByIdAsync(newActivity.TypeId);

            var dtoResult = new ActivityDto(
                newActivity.Id,
                newActivity.Name,
                activityType?.Name ?? "Unknown",
                newActivity.Description,
                newActivity.StartDate,
                newActivity.EndDate,
                newActivity.TypeId,
                newActivity.ModuleId
            );

            return ResultDto<ActivityDto>.Success(dtoResult);            
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

        public async Task<ResultDto<bool>> DeleteActivityAsync(Guid activityId)
        {
            var activity = await _repository.GetActivityByIdAsync(activityId, trackChanges: false);
            if (activity == null)
                return ResultDto<bool>.Failed(new ErrorDto
                {
                    Code = "ActivityNotFound",
                    Description = $"Activity with id {activityId} was not found."
                });

            _repository.Delete(activity);
            await _unitOfWork.CompleteAsync();

            return ResultDto<bool>.Success(true);
        }
        public async Task<ActivityDto> UpdateActivityAsync(Guid activityId, UpdateActivityDto dto)
        {
            if (dto.EndDate < dto.StartDate)
            {
                throw new BadRequestException("End date must be after start date.", "Invalid Dates");
            }

            var activity = await _repository.GetActivityByIdAsync(activityId, trackChanges: true);
            if (activity == null)
            {
                throw new NotFoundException($"Activity with id {activityId} was not found.", "Activity Not Found");
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
                activity.TypeId,
                activity.ModuleId
            );
        }
    }
}