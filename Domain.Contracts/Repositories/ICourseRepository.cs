
using Domain.Models.Entities;

namespace Domain.Contracts.Repositories;

public interface ICourseRepository
{
    Task<Course?> GetCourseByIdAsync(Guid courseId, bool trackChanges = false);
}
