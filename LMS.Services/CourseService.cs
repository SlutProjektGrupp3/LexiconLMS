using Domain.Contracts.Repositories;
using LMS.Shared.DTOs.Course;
using LMS.Shared.DTOs.Module;
using Service.Contracts;

namespace LMS.Services
{
    public class CourseService : ICourseService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CourseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }
        public async Task<CourseDetailsDto?> GetCourseByIdAsync(Guid id)
        {
            var course = await _unitOfWork.Courses.GetCourseByIdAsync(id);
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
}
