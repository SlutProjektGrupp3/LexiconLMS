using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using Domain.Models.Exceptions;
using LMS.Shared.DTOs;
using LMS.Shared.DTOs.Course;
using LMS.Shared.DTOs.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

    public async Task<(IEnumerable<CourseDetailsDto> Items, int TotalCount)> GetCourseSummariesAsync(string? search = null, bool? active = null, int page = 1, int pageSize = 12)
    {
        // Build base query
        var repoQuery = (_uow.CourseRepository as Domain.Contracts.Repositories.ICourseRepository)!.GetCourseQuery(false);
        var query = repoQuery;

        if (!string.IsNullOrWhiteSpace(search))
        {
            var lower = search.Trim().ToLower();
            query = query.Where(c => c.Name.ToLower().Contains(lower) || (c.Description != null && c.Description.ToLower().Contains(lower)));
        }

        if (active.HasValue)
        {
            if (active.Value)
                query = query.Where(c => c.EndDate > DateTime.Now);
            else
                query = query.Where(c => c.EndDate <= DateTime.Now);
        }

        var total = await query.CountAsync(CancellationToken.None);

        var items = await query
            .OrderBy(c => c.StartDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ProjectTo<CourseDetailsDto>(_mapper.ConfigurationProvider)
            .ToListAsync(CancellationToken.None);

        return (items, total);
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

    public async Task<ResultDto<CourseDto>> CreateCourseAsync(CreateCourseDto dto)
    {
        var errors = new List<ErrorDto>();

        if (string.IsNullOrWhiteSpace(dto.Name))
            errors.Add(new ErrorDto { Code = "InvalidName", Description = "Name is required." });

        if (dto.StartDate == default)
            errors.Add(new ErrorDto { Code = "InvalidStartDate", Description = "Start date is required." });

        if (dto.EndDate == default)
            errors.Add(new ErrorDto { Code = "InvalidEndDate", Description = "End date is required." });

        if (dto.EndDate <= dto.StartDate)
            errors.Add(new ErrorDto { Code = "InvalidDates", Description = "End date must be after start date." });

        if (dto.StartDate < DateTime.Today)
            errors.Add(new ErrorDto { Code = "InvalidStartDate", Description = "Start date cannot be in the past." });

        if (errors.Any())
            return ResultDto<CourseDto>.Failed(errors);

        var course = _mapper.Map<Course>(dto);

        _uow.CourseRepository.CreateCourse(course);

        await _uow.CompleteAsync();

        var courseDto = _mapper.Map<CourseDto>(course);

        return ResultDto<CourseDto>.Success(courseDto);
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
    public async Task<ResultDto> DeleteCourseAsync(Guid id, bool trackChanges)
    {
        var courseEntity = await _uow.CourseRepository.GetCourseByIdAsync(id, trackChanges);

        if (courseEntity is null)
        {
            return ResultDto.Failed(new ErrorDto
            {
                Code = "CourseNotFound",
                Description = $"Course with id {id} was not found."
            });
        }
        _uow.CourseRepository.Delete(courseEntity);
        await _uow.CompleteAsync();

        return ResultDto.Success;
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