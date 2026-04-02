using AutoMapper;
using Domain.Models.Entities;
using LMS.Shared.DTOs.AuthDtos;
using LMS.Shared.DTOs.Course;
using LMS.Shared.DTOs.CourseDtos;
using LMS.Shared.DTOs.Module;
using LMS.Shared.DTOs.Modules;
using LMS.Shared.DTOs.User;
using LMS.Shared.DTOs;

namespace LMS.Infractructure.Data;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<UserRegistrationDto, ApplicationUser>();
        CreateMap<Course, CourseDto>();
        CreateMap<Course, CourseDetailsDto>();
        CreateMap<CourseCreateDto, Course>();
        CreateMap<CreateModuleDto, Module>();
        CreateMap<Module, ModuleDto>();
        CreateMap<UpdateCourseDto, Course>();
        CreateMap<ApplicationUser, UserDto>();
        CreateMap<ApplicationUser, AvailableStudentDto>();

        // Explicit mapping for Module -> ModuleDto record
        CreateMap<Module, ModuleDto>()
            .ForCtorParam("id", opt => opt.MapFrom(src => src.Id))
            .ForCtorParam("name", opt => opt.MapFrom(src => src.Name))
            .ForCtorParam("description", opt => opt.MapFrom(src => src.Description))
            .ForCtorParam("startDate", opt => opt.MapFrom(src => src.StartDate))
            .ForCtorParam("endDate", opt => opt.MapFrom(src => src.EndDate))
            .ForCtorParam("courseId", opt => opt.MapFrom(src => src.CourseId))
            .ForCtorParam("links", opt => opt.MapFrom(_ => new List<LinkDto>()));

        CreateMap<Course, StudentMyCourseDto>()
            .ForMember(dest => dest.Modules, opt => opt.MapFrom(src => src.Modules));
    }
}
