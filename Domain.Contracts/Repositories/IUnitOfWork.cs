using Service.Contracts;

namespace Domain.Contracts.Repositories;

public interface IUnitOfWork
{
    ICourseRepository CourseRepository { get; }
    Task CompleteAsync();
}