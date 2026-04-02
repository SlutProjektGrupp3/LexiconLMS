namespace Domain.Models.Entities
{
    public class ActivityType
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public ICollection<ModuleActivity> Activities { get; set; } = new List<ModuleActivity>();

        //Lecture,
        //Assignment,
        //Exercise,
        //Discussion,
        //Project
    }
}