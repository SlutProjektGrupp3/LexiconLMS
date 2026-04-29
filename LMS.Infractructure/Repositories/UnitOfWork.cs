using Domain.Contracts.Repositories;
using LMS.Infractructure.Data;
using Service.Contracts;

namespace LMS.Infractructure.Repositories;
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext context;
    private readonly Lazy<ICourseRepository> courseRepository;
    private readonly Lazy<IModuleRepository> moduleRepository;
    private readonly Lazy<IUserRepository> userRepository;
    private readonly Lazy<IStudentRepository> studentRepository;
    private readonly Lazy<IActivityRepository> activityRepository;
    public ICourseRepository CourseRepository => courseRepository.Value;
    public IModuleRepository ModuleRepository => moduleRepository.Value;
    public IUserRepository UserRepository => userRepository.Value;
    public IStudentRepository StudentRepository => studentRepository.Value;
    public IActivityRepository ActivityRepository => activityRepository.Value;

    public UnitOfWork(
        ApplicationDbContext context,
        Lazy<ICourseRepository> courseRepository,
        Lazy<IModuleRepository> moduleRepository,
        Lazy<IUserRepository> userRepository,
        Lazy<IStudentRepository> studentRepository,
        Lazy<IActivityRepository> activityRepository
        )
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
        this.courseRepository = courseRepository ?? throw new ArgumentNullException(nameof(courseRepository));
        this.moduleRepository = moduleRepository ?? throw new ArgumentNullException(nameof(moduleRepository));
        this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        this.studentRepository = studentRepository ?? throw new ArgumentNullException(nameof(studentRepository));
        this.activityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
    }

    public async Task CompleteAsync() => await context.SaveChangesAsync();
}

