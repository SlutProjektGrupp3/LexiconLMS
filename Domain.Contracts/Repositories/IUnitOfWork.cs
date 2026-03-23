namespace Domain.Contracts.Repositories;

public interface IUnitOfWork
{
    ICourseRepository Courses { get; }
    IUserRepository Users { get; }
    Task CompleteAsync();
}