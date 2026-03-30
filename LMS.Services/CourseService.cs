using AutoMapper;
using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Shared.DTOs;
using LMS.Shared.DTOs.CourseDtos;
using LMS.Shared.DTOs.Course;
using LMS.Shared.DTOs.Module;
using Service.Contracts;


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

    public async Task<CourseDetailsDto?> GetCourseByIdAsync(Guid id)
    {
        var course = await uow.CourseRepository.GetCourseByIdAsync(id, includeModules: true);
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

    public async Task<CourseDto> CreateCourseAsync(CreateCourseDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Name is required.");

        if (dto.StartDate == default)
            throw new ArgumentException("Start date is required.");

        if (dto.EndDate == default)
            throw new ArgumentException("End date is required.");

        if (dto.EndDate <= dto.StartDate)
            throw new ArgumentException("End date must be after start date.");

        if (dto.StartDate < DateTime.Today)
            throw new ArgumentException("Start date cannot be in the past.");

        var course = new Course
        {
            Name = dto.Name,
            Description = dto.Description,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
        };

        uow.CourseRepository.CreateCourse(course);

        await uow.CompleteAsync();

        return new CourseDto
        {
            Id = course.Id,
            Name = course.Name,
            Description = course.Description,
            StartDate = course.StartDate,
            EndDate = course.EndDate
        };
    }

    public async Task UpdateCourseAsync(Guid id, UpdateCourseDto updateCourseDto, bool trackChanges)
    {
        var courseEntity = await uow.CourseRepository.GetCourseByIdAsync(id, trackChanges, includeModules: false);

        if (courseEntity is null)
            throw new KeyNotFoundException($"Course with id {id} was not found.");

        if (updateCourseDto.EndDate < updateCourseDto.StartDate)
            throw new Exception("End date must be after start date.");

        mapper.Map(updateCourseDto, courseEntity);

        await uow.CompleteAsync();
    }
    public async Task DeleteCourseAsync(Guid id, bool trackChanges)
    {
        var courseEntity = await uow.CourseRepository.GetCourseByIdAsync(id, trackChanges);

        if (courseEntity is null)
            throw new Exception($"Course with id {id} was not found.");

        uow.CourseRepository.Delete(courseEntity);
        await uow.CompleteAsync();
    }

    public async Task<IEnumerable<ParticipantDto>> GetParticipantsAsync(Guid courseId)
    {
        var course = await uow.CourseRepository
            .GetCourseWithStudentsAsync(courseId, trackChanges: false);

        if (course is null)
            return Enumerable.Empty<ParticipantDto>();

        return course.Students.Select(s => new ParticipantDto(
            Guid.Parse(s.Id),
            s.FirstName,
            s.LastName,
            s.Email!
        ));
    }
}