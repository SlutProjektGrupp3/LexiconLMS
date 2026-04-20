using AutoMapper;
using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using Domain.Models.Exceptions;
using LMS.Shared.DTOs;
using LMS.Shared.DTOs.Activity;
using Service.Contracts;

namespace LMS.Services;

public class ActivityService : IActivityService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ActivityService( IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ActivityDto>> GetActivitiesForModuleAsync(Guid moduleId)
    {
        var activitiesFromDb = await _unitOfWork.ActivityRepository.GetActivitiesByModuleIdAsync(moduleId, trackChanges: false);

        return _mapper.Map<IEnumerable<ActivityDto>>(activitiesFromDb);
    }

    public async Task<ActivityDto> CreateActivityAsync(CreateActivityDto dto)
    {
        var moduleExists = await _unitOfWork.ActivityRepository.ModuleExistsAsync(dto.ModuleId);

        if (!moduleExists)
            throw new NotFoundException($"Module with id {dto.ModuleId} was not found.");

        var newActivity = _mapper.Map<ModuleActivity>(dto);

        _unitOfWork.ActivityRepository.Create(newActivity);
        await _unitOfWork.CompleteAsync();

        var dtoResult = _mapper.Map<ActivityDto>(newActivity);

        return dtoResult;
    }

    public async Task<IEnumerable<ActivityTypeDto>> GetAllActivityTypesAsync()
    {
        var typesFromDb = await _unitOfWork.ActivityRepository.GetAllActivityTypesAsync(trackChanges: false);

        return _mapper.Map<IEnumerable<ActivityTypeDto>>(typesFromDb);
    }
    public async Task DeleteActivityAsync(Guid activityId)
    {
        var activity = await _unitOfWork.ActivityRepository
            .GetActivityByIdAsync(activityId, trackChanges: false);

        if (activity == null)
            throw new NotFoundException($"Activity with id {activityId} was not found.");

        _unitOfWork.ActivityRepository.Delete(activity);
        await _unitOfWork.CompleteAsync();
        
    }
    public async Task<ActivityDto> UpdateActivityAsync(Guid activityId, UpdateActivityDto dto)
    {
        var activity = await _unitOfWork.ActivityRepository.GetActivityByIdAsync(activityId, trackChanges: true);
        if (activity == null)
            throw new NotFoundException($"Activity with id {activityId} was not found.");

        _mapper.Map(dto, activity);

        await _unitOfWork.CompleteAsync();

        var dtoResult = new ActivityDto(
                activity.Id,
                activity.Name,
                activity.Type?.Name ?? "Unknown",
                activity.Description,
                activity.StartDate,
                activity.EndDate,
                activity.TypeId,
                activity.ModuleId
            );

        return dtoResult;
    }
}