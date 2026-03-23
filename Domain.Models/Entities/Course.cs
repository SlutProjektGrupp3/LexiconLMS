namespace Domain.Models.Entities;

public class Course
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public ICollection<ApplicationUser> Students { get; set; } = new List<ApplicationUser>();
    public ICollection<Module> Modules { get; set; } = new List<Module>();
}