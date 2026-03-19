namespace Domain.Contracts.Repositories;

public interface IUnitOfWork
{
    IModuleRepository ModuleRepository { get; }

    Task CompleteAsync();
}
