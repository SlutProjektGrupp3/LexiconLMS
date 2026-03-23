using Domain.Contracts.Repositories;
using LMS.Infractructure.Data;

namespace LMS.Infractructure.Repositories;
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext context;

    public ICourseRepository Courses { get; }
    public IUserRepository Users { get; }

    public UnitOfWork(ApplicationDbContext context, ICourseRepository courses, IUserRepository users)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
        Courses = courses;
        Users = users;
    }

    public async Task CompleteAsync() => await context.SaveChangesAsync();
}
