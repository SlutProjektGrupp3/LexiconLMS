using Domain.Models.Entities;

namespace Domain.Contracts.Repositories
{
    public interface IModuleRepository : IRepositoryBase<Module>
    {
       
        Task<Module?> GetModuleByIdAndCourseIdAsync(Guid moduleId, Guid courseId, bool trackChanges = false);

    }
}
