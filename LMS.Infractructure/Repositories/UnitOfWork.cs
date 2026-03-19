using Domain.Contracts.Repositories;
using LMS.Infractructure.Data;

namespace LMS.Infractructure.Repositories;
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext context;

    public ICourseRepository Courses { get; }

    public UnitOfWork(ApplicationDbContext context, ICourseRepository courses)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
        Courses = courses;
    }

    public async Task CompleteAsync() => await context.SaveChangesAsync();
}
