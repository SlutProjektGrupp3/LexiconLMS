using AutoMapper;
using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Shared.DTOs.Activity;
using Service.Contracts;

namespace LMS.Services;

public class ActivityService : IActivityService
{
    private readonly IActivityRepository repository;
    private readonly IUnitOfWork unitOfWork;
    private readonly IMapper mapper;

    public ActivityService(IActivityRepository repository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        this.repository = repository;
        this.unitOfWork = unitOfWork;
        this.mapper = mapper;
    }

    public async Task<IEnumerable<ActivityDto>> GetActivitiesForModuleAsync(Guid moduleId)
    {
        var activitiesFromDb = await repository.GetActivitiesByModuleIdAsync(moduleId, trackChanges: false);

        return mapper.Map<IEnumerable<ActivityDto>>(activitiesFromDb);
    }

    public async Task<ActivityDto> CreateActivityAsync(CreateActivityDto dto)
    {
        var newActivity = mapper.Map<ModuleActivity>(dto);

        repository.Create(newActivity);
        await unitOfWork.CompleteAsync();
        var activityType = await repository.GetActivityTypeByIdAsync(newActivity.TypeId);

        return mapper.Map<ActivityDto>(newActivity);
    }

    public async Task<IEnumerable<ActivityTypeDto>> GetAllActivityTypesAsync()
    {
        var typesFromDb = await repository.GetAllActivityTypesAsync(trackChanges: false);

        return typesFromDb.Select(t => new ActivityTypeDto
        {
            Id = t.Id,
            Name = t.Name
        }).ToList();
    }

    public async Task DeleteActivityAsync(Guid activityId)
    {
        var activity = await repository.GetActivityByIdAsync(activityId, trackChanges: false);
        if (activity != null)
        {
            repository.Delete(activity); 
            await unitOfWork.CompleteAsync();
        }
    }
    public async Task<ActivityDto> UpdateActivityAsync(Guid activityId, UpdateActivityDto dto)
    {
        var activity = await repository.GetActivityByIdAsync(activityId, trackChanges: true);

        mapper.Map(dto, activity);

        await unitOfWork.CompleteAsync();

        var activityType = await repository.GetActivityTypeByIdAsync(activity.TypeId);

        return mapper.Map<ActivityDto>(activity);
    }
}