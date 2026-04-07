using AutoMapper;
using Domain.Models.Entities;
using LMS.Shared.DTOs.AuthDtos;
using LMS.Shared.DTOs.Course;
using LMS.Shared.DTOs.Module;
using LMS.Shared.DTOs.Activity;

namespace LMS.Infractructure.Data;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        //User mappings
        CreateMap<UserRegistrationDto, ApplicationUser>();
        CreateMap<ApplicationUser, ParticipantDto>();

        //Course mappings
        CreateMap<Course, CourseDto>();
        CreateMap<CreateCourseDto, Course>();
        CreateMap<Course, CourseDetailsDto>();
        CreateMap<UpdateCourseDto, Course>();

        //Module mappings
        CreateMap<Module, ModuleDto>();
        CreateMap<CreateModuleDto, Module>();
        CreateMap<UpdateModuleDto, Module>();

        //Activity mappings
        CreateMap<ModuleActivity, ActivityDto>();
        CreateMap<CreateActivityDto, ModuleActivity>();
        CreateMap<UpdateActivityDto, ModuleActivity>();
        CreateMap<ActivityType, ActivityTypeDto>();

    }
}
