using Bogus;
using Domain.Models.Entities;
using LMS.Infractructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LMS.API.Services;

//You need all this for JWT to work :) 
//User Secrets Json
//Important to have secretkey inside same key "JwtSettings" as used in appsettings.json for get both sections!!!!
//{
//     "password": "YourSecretPasswordHere",
//     "JwtSettings": {
//        "secretkey": "ThisMustBeReallyLong!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!"
//        }
//}
public class DataSeedHostingService : IHostedService
{
    private readonly IServiceProvider serviceProvider;
    private readonly IConfiguration configuration;
    private readonly ILogger<DataSeedHostingService> logger;
    private UserManager<ApplicationUser> userManager = null!;
    private RoleManager<IdentityRole> roleManager = null!;
    private const string TeacherRole = "Teacher";
    private const string StudentRole = "Student";

    public DataSeedHostingService(IServiceProvider serviceProvider, IConfiguration configuration, ILogger<DataSeedHostingService> logger)
    {
        this.serviceProvider = serviceProvider;
        this.configuration = configuration;
        this.logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();

        var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
        if (!env.IsDevelopment()) return;

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        if (await context.Users.AnyAsync(cancellationToken)) return;

        userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        ArgumentNullException.ThrowIfNull(roleManager, nameof(roleManager));
        ArgumentNullException.ThrowIfNull(userManager, nameof(userManager));

        try
        {
            await context.Database.MigrateAsync(cancellationToken);

            await AddRolesAsync([TeacherRole, StudentRole]);
            await AddDemoUsersAsync();

            await AddStudentsAsync(20);

            var activityTypes = await AddActivityTypesAsync(context);

            //await AddCoursesAsync(context);
            await AddCoursesWithDependenciesAsync(context, activityTypes);

            //var course = await AddCourseWithModulesAsync(context, cancellationToken);

            logger.LogInformation("Seed complete");
        }
        catch (Exception ex)
        {
            logger.LogError($"Data seed fail with error: {ex.Message}");
            throw;
        }
    }

    private async Task AddRolesAsync(string[] rolenames)
    {
        foreach (string rolename in rolenames)
        {
            if (await roleManager.RoleExistsAsync(rolename)) continue;
            var role = new IdentityRole { Name = rolename };
            var res = await roleManager.CreateAsync(role);

            if (!res.Succeeded) throw new Exception(string.Join("\n", res.Errors));
        }
    }
    private async Task AddDemoUsersAsync()
    {
        var teacher = new ApplicationUser
        {
            FirstName = "Teacher",
            LastName = "Testsson",
            UserName = "teacher@test.com",
            Email = "teacher@test.com"
        };

        var teacher2 = new ApplicationUser
        {
            FirstName = "Teacher2",
            LastName = "Testsson",
            UserName = "teacher2@test.com",
            Email = "teacher2@test.com"
        };

        var student = new ApplicationUser
        {
            FirstName = "Student",
            LastName = "Testsson",
            UserName = "student@test.com",
            Email = "student@test.com"
        };

        var student2 = new ApplicationUser
        {
            FirstName = "Student2",
            LastName = "Testsson",
            UserName = "student2@test.com",
            Email = "student2@test.com",
        };


        await AddUserToDb([teacher, teacher2, student, student2]);

        var teacherRoleResult = await userManager.AddToRoleAsync(teacher, TeacherRole);
        if (!teacherRoleResult.Succeeded) throw new Exception(string.Join("\n", teacherRoleResult.Errors));

        var teacherRoleResult2 = await userManager.AddToRoleAsync(teacher2, TeacherRole);
        if (!teacherRoleResult2.Succeeded) throw new Exception(string.Join("\n", teacherRoleResult2.Errors));

        var studentRoleResult = await userManager.AddToRoleAsync(student, StudentRole);
        if (!studentRoleResult.Succeeded) throw new Exception(string.Join("\n", studentRoleResult.Errors));

        var studentRoleResult2 = await userManager.AddToRoleAsync(student2, StudentRole);
        if (!studentRoleResult2.Succeeded) throw new Exception(string.Join("\n", studentRoleResult2.Errors));
    }    

    private async Task AddStudentsAsync(int numberOfStudents)
    {
        var faker = new Faker<ApplicationUser>("en").Rules((f, u) =>
        {
            u.FirstName = f.Person.FirstName;
            u.LastName = f.Person.LastName;
            u.Email = f.Internet.Email(u.FirstName, u.LastName);
            u.UserName = u.Email;
        });

        var students = faker.Generate(numberOfStudents);

        var password = configuration["password"];
        ArgumentNullException.ThrowIfNull(password);

        foreach (var student in students)
        {
            var result = await userManager.CreateAsync(student, password);

            if (!result.Succeeded)
                throw new Exception(string.Join("\n", result.Errors));

            var roleResult = await userManager.AddToRoleAsync(student, StudentRole);

            if (!roleResult.Succeeded)
                throw new Exception(string.Join("\n", roleResult.Errors));
        }
    }
    private async Task<List<ActivityType>> AddActivityTypesAsync(ApplicationDbContext context)
    {
        if (await context.ActivityTypes.AnyAsync())
            return await context.ActivityTypes.ToListAsync();

        var types = new List<ActivityType>
        {
            new() { Id = Guid.NewGuid(), Name = "Lecture" },
            new() { Id = Guid.NewGuid(), Name = "Assignment" },
            new() { Id = Guid.NewGuid(), Name = "Exercise" },
            new() { Id = Guid.NewGuid(), Name = "Quiz" },
            new() { Id = Guid.NewGuid(), Name = "Project" }
        };

        context.ActivityTypes.AddRange(types);
        await context.SaveChangesAsync();
        return types;
    }
    private async Task AddCoursesWithDependenciesAsync(ApplicationDbContext context, List<ActivityType> types)
    {
        var faker = new Faker("en");
        var courses = new List<Course>();

        var courseNames = new[] { "C# Backend", "ASP.NET Core", "EF Core Essentials", "Fullstack Web" };

        for (int i = 0; i < 3; i++)
        {
            var courseId = Guid.NewGuid();
            var course = new Course
            {
                Id = courseId,
                Name = courseNames[i % courseNames.Length],
                Description = faker.Lorem.Sentence(10),
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(3),
                Modules = GenerateModules(faker, courseId, types)
            };
            courses.Add(course);
        }

        context.Courses.AddRange(courses);
        await context.SaveChangesAsync();

        var students = await userManager.GetUsersInRoleAsync(StudentRole);
        foreach (var student in students)
        {
            student.CourseId = faker.PickRandom(courses).Id;
        }
        await context.SaveChangesAsync();
    }
    private ICollection<Module> GenerateModules(Faker faker, Guid courseId, List<ActivityType> types)
    {
        var modules = new List<Module>();
        int count = faker.Random.Int(3, 6);

        for (int i = 0; i < count; i++)
        {
            var moduleId = Guid.NewGuid();
            modules.Add(new Module
            {
                Id = moduleId,
                Name = $"Module: {faker.Commerce.Department()}",
                Description = faker.Lorem.Sentence(5),
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(14),
                CourseId = courseId,
                Activities = GenerateActivities(faker, types)
            });
        }
        return modules;
    }
    private ICollection<ModuleActivity> GenerateActivities(Faker faker, List<ActivityType> types)
    {
        var activities = new List<ModuleActivity>();
        int count = faker.Random.Int(2, 5);

        for (int i = 0; i < count; i++)
        {
            activities.Add(new ModuleActivity
            {
                Id = Guid.NewGuid(),
                Name = faker.Lorem.Word(),
                Description = faker.Lorem.Sentence(5),
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(3),
                TypeId = faker.PickRandom(types).Id
            });
        }
        return activities;
    }
    private async Task AddCoursesAsync(ApplicationDbContext context)
    {
        if (await context.Courses.AnyAsync()) return;

        var faker = new Faker("en");        
        int courseCount = 3;
        var courses = new List<Course>();

        var courseNames = new[]
        {
            "C# Backend Development",
            "ASP.NET Core Advanced",
            "Entity Framework Core Essentials",
            "Fullstack Web Development",
            "Database Design and Optimization"
        };

        var courseDescriptions = new[]
        {
            "Learn backend development using C# and ASP.NET Core.",
            "Build scalable web APIs with proper architecture patterns.",
            "Understand database management and EF Core migrations.",
            "Practice fullstack development with frontend and backend.",
            "Master relational database design and optimization techniques."
        };

        for (int i = 0; i < courseCount; i++)
        {
            var courseId = Guid.NewGuid();
            var startDate = DateTime.UtcNow;
            var endDate = startDate.AddMonths(faker.Random.Int(2, 4));

            var course = new Course
            {
                Id = courseId,
                Name = faker.PickRandom(courseNames),
                Description = faker.PickRandom(courseDescriptions),
                StartDate = startDate,
                EndDate = endDate,
                Modules = GenerateModules(faker, courseId)
            };

            courses.Add(course);
        }

        context.Courses.AddRange(courses);
        await context.SaveChangesAsync();

        var students = await userManager.GetUsersInRoleAsync(StudentRole);
        var rand = new Random();
        foreach (var student in students)
        {
            student.CourseId = courses[rand.Next(courses.Count)].Id;
        }

        await context.SaveChangesAsync();
    }

    private ICollection<Module> GenerateModules(Faker faker, Guid courseId)
    {
        int moduleCount = faker.Random.Int(5, 15); 
        var modules = new List<Module>();

        for (int i = 0; i < moduleCount; i++)
        {
            var moduleId = Guid.NewGuid();
            var startDate = DateTime.UtcNow;
            var endDate = startDate.AddMonths(faker.Random.Int(1, 2));

            var module = new Module
            {
                Id = moduleId,
                Name = faker.Company.CatchPhrase(), 
                Description = faker.Lorem.Sentence(8),
                StartDate = startDate,
                EndDate = endDate,
                CourseId = courseId,
                Activities = GenerateActivities(faker)
            };

            modules.Add(module);
        }

        return modules;
    }

    private ICollection<ModuleActivity> GenerateActivities(Faker faker)
    {
        int activityCount = faker.Random.Int(3, 20);
        var activities = new List<ModuleActivity>();

        for (int i = 0; i < activityCount; i++)
        {
            activities.Add(new ModuleActivity
            {
                Id = Guid.NewGuid(),
                Type = new ActivityType
                {
                    Name = faker.PickRandom(new[] { "Assignment", "Exercise", "Quiz" })
                },
                Name = faker.Lorem.Word(), 
                Description = faker.Lorem.Sentence(6), 
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(faker.Random.Int(1, 10))
            });
        }

        return activities;
    }

    private async Task AddUserToDb(IEnumerable<ApplicationUser> users)
    {
        var passWord = configuration["password"];
        ArgumentNullException.ThrowIfNull(passWord, nameof(passWord));

        foreach (var user in users)
        {
            var result = await userManager.CreateAsync(user, passWord);
            if (!result.Succeeded) throw new Exception(string.Join("\n", result.Errors));
        }
    }
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

}
