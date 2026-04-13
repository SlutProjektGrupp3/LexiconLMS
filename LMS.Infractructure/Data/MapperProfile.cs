using AutoMapper;
using Domain.Models.Entities;
using LMS.Shared.DTOs.AuthDtos;
using LMS.Shared.DTOs.Course;
using LMS.Shared.DTOs.Module;
using LMS.Shared.DTOs.Activity;
using LMS.Shared.DTOs.User;
using System.Collections;

namespace LMS.Infractructure.Data;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        //User mappings
        CreateMap<UserRegistrationDto, ApplicationUser>();
        CreateMap<ApplicationUser, UserDto>();
        CreateMap<ApplicationUser, AvailableStudentDto>();

        //Course mappings
        CreateMap<Course, CourseDto>();
        CreateMap<CreateCourseDto, Course>();

        // Map Course to the richer CourseDetailsDto which now also contains optional summary fields
        CreateMap<Course, CourseDetailsDto>()
            .ForMember(dest => dest.Modules, opt => opt.MapFrom(src => src.Modules))
            .ForMember(dest => dest.ParticipantsCount, opt => opt.MapFrom(src => src.Students.Count))
            .ForMember(dest => dest.ModulesCount, opt => opt.MapFrom(src => src.Modules.Count))
            .ForMember(dest => dest.Active, opt => opt.MapFrom(src => src.EndDate > DateTime.UtcNow))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description ?? string.Empty));
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
