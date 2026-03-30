using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models.Entities
{
    public class ModuleActivity
    {
        public Guid Id { get; set; }        
        public required string Name { get; set; }
        public string Description { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid ModuleId { get; set; }
        public Module? Module { get; set; }
        public Guid TypeId { get; set; }
        public ActivityType? Type { get; set; }
    }
}
