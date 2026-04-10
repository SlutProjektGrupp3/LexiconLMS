using LMS.Shared.DTOs.Module;
using LMS.Shared.DTOs.User;
using System;
using System.Collections.Generic;

namespace LMS.Shared.DTOs.Course
{
    public class CourseSummaryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ParticipantsCount { get; set; }
        public int ModulesCount { get; set; }
        public bool Active { get; set; }
        // optional lists not populated by the summary endpoint
        public List<UserDto>? Participants { get; set; }
        public List<ModuleDto>? Modules { get; set; }
    }
}
