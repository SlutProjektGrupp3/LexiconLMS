namespace Domain.Contracts.Repositories;

public interface IUnitOfWork
{
    IModuleRepository ModuleRepository { get; }

    ICourseRepository Courses { get; }
    Task CompleteAsync();
}
