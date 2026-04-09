using AutoMapper;
using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using Domain.Models.Exceptions;
using LMS.Shared.DTOs;
using LMS.Shared.DTOs.Course;
using LMS.Shared.DTOs.User;
using Microsoft.AspNetCore.Identity;
using Service.Contracts;

namespace LMS.Services;

public class CourseService : ICourseService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;

    public CourseService(ICourseRepository courseRepository,IUnitOfWork uow, IMapper mapper, UserManager<ApplicationUser> userManager)
    {
        _uow = uow;
        _mapper = mapper;
        _userManager = userManager;
    }

    public async Task<IEnumerable<CourseDto>> GetAllCoursesAsync(bool trackChanges = false)
    {
        var courses = await _uow.CourseRepository.GetAllCoursesAsync(trackChanges);
        return _mapper.Map<IEnumerable<CourseDto>>(courses);
    }

    public async Task<CourseDetailsDto> GetCourseByIdAsync(Guid id)
    {
        var course = await _uow.CourseRepository.GetCourseByIdAsync(id, includeModules: true);
        if (course == null)
            throw new NotFoundException($"Course with id {id} was not found.");

        return _mapper.Map<CourseDetailsDto>(course);
    }

    public async Task<CourseDto> CreateCourseAsync(CreateCourseDto dto)
    {
        var course = _mapper.Map<Course>(dto);

        _uow.CourseRepository.CreateCourse(course);

        await _uow.CompleteAsync();

        return _mapper.Map<CourseDto>(course);
    }

    public async Task UpdateCourseAsync(Guid id, UpdateCourseDto updateCourseDto, bool trackChanges)
    {
        var courseEntity = await _uow.CourseRepository
            .GetCourseByIdAsync(id, trackChanges, includeModules: false);

        if (courseEntity is null)
            throw new NotFoundException($"Course with id {id} was not found.");       

        _mapper.Map(updateCourseDto, courseEntity);

        await _uow.CompleteAsync();
    }
    public async Task DeleteCourseAsync(Guid id, bool trackChanges)
    {
        var courseEntity = await _uow.CourseRepository.GetCourseByIdAsync(id, trackChanges);

        if (courseEntity is null)
        {
            throw new NotFoundException($"Course with id {id} was not found.");
        }
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

    public async Task AddStudentToCourseAsync(Guid courseId, string studentId)
    {
        var course = await _uow.CourseRepository
            .GetCourseByIdAsync(courseId, trackChanges: false);

        if (course == null)
            throw new NotFoundException("Course not found.");

        var student = await _userManager.FindByIdAsync(studentId);
        if (student == null)
            throw new NotFoundException("Student not found.");

        var roles = await _userManager.GetRolesAsync(student);
        if (!roles.Contains("Student"))
            throw new BadRequestException("Selected user is not a student.");

        if (student.CourseId == courseId)
            throw new BadRequestException("Student is already enrolled in this course.");

        if (student.CourseId != null && student.CourseId != courseId)
            throw new BadRequestException("Student is already enrolled in another course.");

        student.CourseId = courseId;

        var result = await _userManager.UpdateAsync(student);

        if (!result.Succeeded)
            throw new BadRequestException("Failed to add student to course.");
    }

    public async Task<IEnumerable<AvailableStudentDto>> GetAvailableStudentsAsync()
    {
        var users = await _userManager.GetUsersInRoleAsync("Student");

        return users
            .Where(u => u.CourseId == null)
            .Select(u => _mapper.Map<AvailableStudentDto>(u))
            .ToList();
    }


}