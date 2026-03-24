using AutoMapper;
using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Shared.DTOs.CourseDtos;
using LMS.Shared.DTOs.Course;
using LMS.Shared.DTOs.Module;
using Service.Contracts;
using LMS.Shared.Request;

namespace LMS.Services;

public class CourseService : ICourseService
{
    private IUnitOfWork uow;
    private readonly IMapper mapper;

    public CourseService(IUnitOfWork uow, IMapper mapper)
    {
        this.uow = uow;
        this.mapper = mapper;
    }

    public async Task<IEnumerable<CourseDto>> GetAllCoursesAsync(bool trackChanges = false)
    {
        var courses = await uow.CourseRepository.GetAllCoursesAsync(trackChanges);
        return mapper.Map<IEnumerable<CourseDto>>(courses);
    }
    
    public async Task<CourseDto> CreateCourseAsync(CourseCreateDto dto)
    {
        var course = mapper.Map<Course>(dto);

        uow.CourseRepository.Create(course);
        await uow.CompleteAsync();

        return mapper.Map<CourseDto>(course);
    }

    public async Task<CourseDetailsDto?> GetCourseByIdAsync(Guid id)
    {
        var course = await uow.CourseRepository.GetCourseByIdAsync(id);
        if (course == null)
            return null;

        return new CourseDetailsDto(
            course.Id,
            course.Name,
            course.Description,
            course.StartDate,
            course.EndDate,
            course.Modules.Select(m => new ModuleDto(
                m.Id,
                m.Name,
                m.Description,
                m.StartDate,
                m.EndDate
            )).ToList()
        );
    }
}
