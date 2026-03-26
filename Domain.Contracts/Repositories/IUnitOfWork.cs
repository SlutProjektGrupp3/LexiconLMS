using Service.Contracts;

namespace Domain.Contracts.Repositories;

public interface IUnitOfWork
{
    ICourseRepository CourseRepository { get; }
    IModuleRepository ModuleRepository { get; }
    IUserRepository UserRepository { get; }
    Task CompleteAsync();    
}
