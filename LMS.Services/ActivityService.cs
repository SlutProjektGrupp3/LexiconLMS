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
                a.EndDate
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
                Type = new ActivityType
                {
                    Id = Guid.NewGuid(),
                    Name = dto.ActivityTypeName
                }
            };

            _repository.Create(newActivity);
            await _unitOfWork.CompleteAsync();

            return new ActivityDto(
                newActivity.Id,
                newActivity.Name,
                newActivity.Type.Name,
                newActivity.Description,
                newActivity.StartDate,
                newActivity.EndDate
            );
        }
    }

}