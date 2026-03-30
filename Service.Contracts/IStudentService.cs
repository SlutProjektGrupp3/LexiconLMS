using LMS.Shared.DTOs.CourseDtos;

namespace Service.Contracts
{
    public interface IStudentService
    {
        Task<StudentMyCourseDto?> GetMyCourseAsync(Guid userId);
    }
}
