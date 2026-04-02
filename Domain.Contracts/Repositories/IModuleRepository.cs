using Domain.Models.Entities;

namespace Domain.Contracts.Repositories
{
    public interface IModuleRepository : IRepositoryBase<Module>
    {
        Task<Module?> GetModuleByIdAsync(Guid moduleId, bool trackChanges = false, bool includeActivities = false);
       
        Task<Module?> GetModuleByIdAndCourseIdAsync(Guid moduleId, Guid courseId, bool trackChanges = false);

    }
}
