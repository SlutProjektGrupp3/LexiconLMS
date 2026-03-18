
using Domain.Contracts.Repositories;
using Domain.Models.Entities;
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

    public async Task<Course> CreateCourseAsync(CreateCourseDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Namn måste anges.");

        if (dto.StartDate == default)
            throw new ArgumentException("Startdatum måste anges.");

        if (dto.EndDate == default)
            throw new ArgumentException("Slutdatum måste anges.");

        if (dto.EndDate <= dto.StartDate)
            throw new ArgumentException("Slutdatum måste vara efter startdatum.");

        if (dto.StartDate < DateTime.UtcNow.Date)
            throw new ArgumentException("Startdatum kan inte vara i det förflutna.");



        var course = new Course
        {
            Name = dto.Name,
            Description = dto.Description,
           StartDate = dto.StartDate,   
           EndDate = dto.EndDate,
        };

        _courseRepository.CreateCourse(course);

        await _unitOfWork.CompleteAsync();

        return course;
    }
}
