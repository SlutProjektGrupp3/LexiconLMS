using Domain.Contracts.Repositories;
using LMS.Shared.DTOs.CourseDtos;
using LMS.Shared.DTOs.Module;
using Service.Contracts;

namespace LMS.Services
{
    public class StudentService : IStudentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StudentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<StudentMyCourseDto?> GetMyCourseAsync(Guid userId)
        {
            var student = await _unitOfWork.StudentRepository.GetStudentWithCourseAsync(userId, trackChanges: false);

            if (student is null || student.Course is null)
                return null;

            return new StudentMyCourseDto
            {
                Id = student.Course.Id,
                Name = student.Course.Name,
                Description = student.Course.Description,
                StartDate = student.Course.StartDate,
                EndDate = student.Course.EndDate,
                Modules = student.Course.Modules.Select(m => new ModuleDto(
                    m.Id,
                    m.Name,
                    m.Description,
                    m.StartDate,
                    m.EndDate
                )).ToList()
            };
        }
    }
}