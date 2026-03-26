using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Shared.DTOs;
using Service.Contracts.Courses;

namespace Application.Services;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _courseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CourseService(ICourseRepository courseRepository, IUnitOfWork unitOfWork)
    {
        _courseRepository = courseRepository;
        _unitOfWork = unitOfWork;
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

        _courseRepository.CreateCourse(course);

        await _unitOfWork.CompleteAsync();

        return new CourseDto
        {
            Id = course.Id,
            Name = course.Name,
            Description = course.Description,
            StartDate = course.StartDate,
            EndDate = course.EndDate
        };
    }
}