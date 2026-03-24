using AutoMapper;
using Domain.Models.Entities;
using LMS.Shared.DTOs.AuthDtos;
using LMS.Shared.DTOs.Course;
using LMS.Shared.DTOs.CourseDtos;
using LMS.Shared.DTOs.Modules;

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
        CreateMap<UpdateCourseDto, Course>();
    }
}
