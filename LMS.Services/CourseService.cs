using AutoMapper;
using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Shared.DTOs.Course;
using Service.Contracts;

namespace LMS.Services;

public class CourseService : ICourseService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public CourseService(IUnitOfWork uow, IMapper mapper) 
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CourseDto>> GetAllCoursesAsync(bool trackChanges = false)
    {
        var courses = await _uow.CourseRepository.GetAllCoursesAsync(trackChanges);
        return _mapper.Map<IEnumerable<CourseDto>>(courses);
    }

    public async Task<CourseDetailsDto?> GetCourseByIdAsync(Guid id)
    {
        var course = await _uow.CourseRepository.GetCourseByIdAsync(id, includeModules: true);
        if (course == null)
            return null;

        return _mapper.Map<CourseDetailsDto>(course);
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

        var course = _mapper.Map<Course>(dto);

        _uow.CourseRepository.CreateCourse(course);

        await _uow.CompleteAsync();

        return _mapper.Map<CourseDto>(course);
    }

    public async Task UpdateCourseAsync(Guid id, UpdateCourseDto updateCourseDto, bool trackChanges)
    {
        var courseEntity = await _uow.CourseRepository.GetCourseByIdAsync(id, trackChanges, includeModules: false);

        if (courseEntity is null)
            throw new KeyNotFoundException($"Course with id {id} was not found.");

        if (updateCourseDto.EndDate < updateCourseDto.StartDate)
            throw new Exception("End date must be after start date.");

        _mapper.Map(updateCourseDto, courseEntity);

        await _uow.CompleteAsync();
    }
    public async Task DeleteCourseAsync(Guid id, bool trackChanges)
    {
        var courseEntity = await _uow.CourseRepository.GetCourseByIdAsync(id, trackChanges);

        if (courseEntity is null)
            throw new Exception($"Course with id {id} was not found.");

        _uow.CourseRepository.Delete(courseEntity);
        await _uow.CompleteAsync();
    }

    public async Task<IEnumerable<ParticipantDto>> GetParticipantsAsync(Guid courseId)
    {
        var course = await _uow.CourseRepository
            .GetCourseWithStudentsAsync(courseId, trackChanges: false);

        if (course is null)
            return Enumerable.Empty<ParticipantDto>();

        return _mapper.Map<IEnumerable<ParticipantDto>>(course.Students);
    }
}