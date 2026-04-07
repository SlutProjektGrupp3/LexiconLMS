using AutoMapper;
using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Shared.DTOs.Activity;
using Service.Contracts;

namespace LMS.Services;

public class ActivityService : IActivityService
{
    private readonly IActivityRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ActivityService(IActivityRepository repository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ActivityDto>> GetActivitiesForModuleAsync(Guid moduleId)
    {
        var activitiesFromDb = await _repository.GetActivitiesByModuleIdAsync(moduleId, trackChanges: false);

        return _mapper.Map<IEnumerable<ActivityDto>>(activitiesFromDb);
    }

    public async Task<ActivityDto> CreateActivityAsync(CreateActivityDto dto)
    {
        var newActivity = _mapper.Map<ModuleActivity>(dto);

        _repository.Create(newActivity);
        await _unitOfWork.CompleteAsync();
        var activityType = await _repository.GetActivityTypeByIdAsync(newActivity.TypeId);

        return _mapper.Map<ActivityDto>(newActivity);
    }

    public async Task<IEnumerable<ActivityTypeDto>> GetAllActivityTypesAsync()
    {
        var typesFromDb = await _repository.GetAllActivityTypesAsync(trackChanges: false);

        return _mapper.Map<IEnumerable<ActivityTypeDto>>(typesFromDb);
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

        _mapper.Map(dto, activity);

        await _unitOfWork.CompleteAsync();

        var activityType = await _repository.GetActivityTypeByIdAsync(activity.TypeId);

        return _mapper.Map<ActivityDto>(activity);
    }
}