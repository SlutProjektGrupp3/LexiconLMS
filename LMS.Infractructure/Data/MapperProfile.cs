using AutoMapper;
using Domain.Models.Entities;
using LMS.Shared.DTOs.AuthDtos;
using LMS.Shared.DTOs.Course;
using LMS.Shared.DTOs.CourseDtos;
using LMS.Shared.DTOs.Modules;
using LMS.Shared.DTOs.Module;

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
<<<<<<< task-7-3---skapa-ett-formulär-för-att-skapa-en-modul
        CreateMap<Module, ModuleDto>();

        // Explicit mapping for Module -> ModuleDto record
        CreateMap<Module, ModuleDto>()
            .ForCtorParam("id", opt => opt.MapFrom(src => src.Id))
            .ForCtorParam("name", opt => opt.MapFrom(src => src.Name))
            .ForCtorParam("description", opt => opt.MapFrom(src => src.Description))
            .ForCtorParam("startDate", opt => opt.MapFrom(src => src.StartDate))
            .ForCtorParam("endDate", opt => opt.MapFrom(src => src.EndDate));
=======
        CreateMap<UpdateCourseDto, Course>();
>>>>>>> develop
    }
}
