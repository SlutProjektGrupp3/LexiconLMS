using AutoMapper;
using Domain.Models.Entities;
using LMS.Shared.DTOs.AuthDtos;
using LMS.Shared.DTOs.Course;
using LMS.Shared.DTOs.CourseDtos;
using LMS.Shared.DTOs.Module;
using LMS.Shared.DTOs.Modules;
using LMS.Shared.DTOs.Shared;
using LMS.Shared.DTOs.User;

namespace LMS.Infractructure.Data;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<UserRegistrationDto, ApplicationUser>();
        CreateMap<CourseCreateDto, Course>();
        CreateMap<CreateModuleDto, Module>();
        CreateMap<Module, ModuleDto>();
        CreateMap<UpdateCourseDto, Course>();
        CreateMap<ApplicationUser, UserDto>();

        // Explicit mapping for Module -> ModuleDto record
        CreateMap<Module, ModuleDto>()
            .ForCtorParam("id", opt => opt.MapFrom(src => src.Id))
            .ForCtorParam("name", opt => opt.MapFrom(src => src.Name))
            .ForCtorParam("description", opt => opt.MapFrom(src => src.Description))
            .ForCtorParam("startDate", opt => opt.MapFrom(src => src.StartDate))
            .ForCtorParam("endDate", opt => opt.MapFrom(src => src.EndDate));

        CreateMap<Course, StudentMyCourseDto>()
            .ForMember(dest => dest.Modules, opt => opt.MapFrom(src => src.Modules));

        CreateMap<Course, CourseDto>()
            .ForMember(dest => dest.Links, opt => opt.MapFrom(src =>
                new List<LinkDto>
                {
                    new LinkDto { Rel = "self", Href = $"/courses/{src.Id}" },
                    new LinkDto { Rel = "edit", Href = $"/teacher/courses/edit/{src.Id}" },
                    new LinkDto { Rel = "delete", Href = $"/teacher/courses/delete/{src.Id}" },

                    new LinkDto { Rel = "apiSelf", Href = $"/api/courses/{src.Id}" },
                    new LinkDto { Rel = "httpPUT", Href = $"/api/courses/{src.Id}" },
                    new LinkDto { Rel = "httpDELETE", Href = $"/api/courses/{src.Id}" },
                    new LinkDto { Rel = "allCourses", Href = $"api/courses" },
                    new LinkDto { Rel = "createModule", Href = $"/api/modules" },
                }
            ));

        CreateMap<Course, CourseDetailsDto>()
            .ForMember(dest => dest.Links, opt => opt.MapFrom(src =>
                new List<LinkDto>
                {
                    new LinkDto { Rel = "self", Href = $"/courses/{src.Id}" },
                    new LinkDto { Rel = "edit", Href = $"/teacher/courses/edit/{src.Id}" },
                    new LinkDto { Rel = "delete", Href = $"/teacher/courses/delete/{src.Id}" },

                     //new LinkDto { Rel = "GetCourses", Href = Url.Action("GetCourses", "Courses") ?? "#", Method = "GET" },
                    new LinkDto { Rel = "apiSelf", Href = $"/api/courses/{src.Id}" },
                    new LinkDto { Rel = "httpPUT", Href = $"/api/courses/{src.Id}" },
                    new LinkDto { Rel = "httpDELETE", Href = $"/api/courses/{src.Id}" },
                    new LinkDto { Rel = "allCourses", Href = $"api/courses" },
                    new LinkDto { Rel = "createModule", Href = $"/api/modules" },
                }
            ));
    }
}
