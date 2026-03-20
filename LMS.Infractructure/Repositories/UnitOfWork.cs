using Domain.Contracts.Repositories;
using LMS.Infractructure.Data;
using Service.Contracts;

namespace LMS.Infractructure.Repositories;
public class UnitOfWork : IUnitOfWork
{
    private readonly Lazy<ICourseRepository> courseRepository;
    public ICourseRepository CourseRepository => courseRepository.Value;

    private readonly Lazy<IModuleRepository> moduleRepository;
    public IModuleRepository ModuleRepository => moduleRepository.Value;

    private readonly ApplicationDbContext context;

    public UnitOfWork(
        ApplicationDbContext context,
        ICourseRepository courses,
        Lazy<IModuleRepository> moduleRepository
        )
    {
        this.moduleRepository = moduleRepository ?? throw new ArgumentNullException(nameof(moduleRepository));
        this.context = context ?? throw new ArgumentNullException(nameof(context));
        this.courseRepository = courseRepository ?? throw new ArgumentNullException(nameof(courseRepository));
    }

    public async Task CompleteAsync() => await context.SaveChangesAsync();
}

