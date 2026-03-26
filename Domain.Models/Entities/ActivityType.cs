namespace Domain.Models.Entities
{
    public class ActivityType
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }

        //Lecture,
        //Assignment,
        //Exercise,
        //Discussion,
        //Project
    }
}