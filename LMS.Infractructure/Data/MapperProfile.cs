using AutoMapper;
using Domain.Models.Entities;
using LMS.Shared.DTOs.AuthDtos;
using LMS.Shared.DTOs.Course;
using LMS.Shared.DTOs.Module;
using LMS.Shared.DTOs.Activity;
using LMS.Shared.DTOs.User;

namespace LMS.Infractructure.Data;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        //User mappings
        CreateMap<UserRegistrationDto, ApplicationUser>();
        CreateMap<ApplicationUser, ParticipantDto>();
        CreateMap<ApplicationUser, UserDto>();
        CreateMap<ApplicationUser, AvailableStudentDto>();

        //Course mappings
        CreateMap<Course, CourseDto>();
        CreateMap<CreateCourseDto, Course>();
        CreateMap<Course, CourseDetailsDto>();
        CreateMap<UpdateCourseDto, Course>();
        CreateMap<Course, CourseDetailsDto>()
            .ForMember(dest => dest.ModulesCount,
            opt => opt.MapFrom(src => src.Modules.Count()))
            
            .ForMember(dest => dest.ParticipantsCount,
            opt => opt.MapFrom(src => src.Students.Count()));

        //Module mappings
        CreateMap<Module, ModuleDto>();
        CreateMap<CreateModuleDto, Module>();
        CreateMap<UpdateModuleDto, Module>();
        CreateMap<UpdateCourseDto, Course>();

        //Activity mappings
        CreateMap<ModuleActivity, ActivityDto>();
        CreateMap<CreateActivityDto, ModuleActivity>();
        CreateMap<UpdateActivityDto, ModuleActivity>();
        CreateMap<ActivityType, ActivityTypeDto>();

    }
}
