using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models.Entities
{
    public class Module
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string Description { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid CourseId { get; set; }
        public ICollection<ModuleActivity> Activities { get; set; } = new List<ModuleActivity>();

    }
}
