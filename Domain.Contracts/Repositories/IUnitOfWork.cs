namespace Domain.Contracts.Repositories;

public interface IUnitOfWork
{
    ICourseRepository Courses { get; }
    Task CompleteAsync();
}